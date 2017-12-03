using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class UIManager : MBehavior {

	static private UIManager m_Instance;
	static public UIManager Instance {
		get {
			if (m_Instance == null)
				m_Instance = FindObjectOfType<UIManager> ();
			return m_Instance;
		}
	}

	[TabGroup("Element" , true )]
	[SerializeField] Text scoreText;
	[TabGroup("Element" , true )]
	[SerializeField][ReadOnly] float temScore=0;

	[TabGroup("Anchor" , true )]
	[SerializeField] public RectTransform Root;
	[TabGroup("Anchor" , true )]
	[SerializeField] RectTransform NineGridRoot;
	[TabGroup("Anchor" , true )]
	[ReadOnly][SerializeField] List<RectTransform> gridArray = new List<RectTransform>();
	[SerializeField] int gridSize = 3;
	public int GridSize{ get { return gridSize; } }

	[System.Serializable]
	public class PrefabInfo
	{
		public ButtonType type;
		public GameObject prefab;
	}

	[TabGroup("Prefab" , true )]
	[SerializeField][ReadOnly] List<PrefabInfo> buttonPrefabList;

	[SerializeField][ReadOnly] public List<UIElement> temUIElement = new List<UIElement>();

	[System.Serializable]
	public class GridInfo
	{
		public bool IsControlled{
			get {
				return linkEle != null;
			}
		}
		public int x;
		public int y;
		public RectTransform trans;
		public UIElement linkEle;

		public void CleanEle()
		{
			linkEle = null;
		}

		public void LinkEle( UIElement _e )
		{
//			Debug.Log ("Link " + x + " " + y + " " + _e.name);
			linkEle = _e;
		}

	}
	[ReadOnly] public List<List<GridInfo>> gridInfo = new List<List<GridInfo>>();


	public RectTransform GetGirdNine( int i , int j )
	{
		if ( gridArray.Count < gridSize * gridSize )
			InitGrid ();

		int ii = Mathf.Clamp (i, 0, gridSize-1);
		int jj = Mathf.Clamp (j, 0, gridSize-1);

		int index = ii * gridSize + jj;

		return gridArray [index];
	}

	public GridInfo GetGridInfo( int i , int j ) 
	{
		if (gridInfo.Count < gridSize)
			InitGrid ();

		int ii = Mathf.Clamp (i, 0, gridSize-1);
		int jj = Mathf.Clamp (j, 0, gridSize-1);

		return gridInfo [i] [j];
	}

	public void LinkGridInfo( UIElement ele , int i , int j )
	{
		gridInfo [i] [j].LinkEle (ele);
	}

	public bool IsGridAvaiable( int i , int j )
	{
		return !GetGridInfo (i, j).IsControlled;
	}

	public bool IsGridFull ()
	{
		for (int i = 0; i < gridSize; ++i) {
			for (int j = 0; j < gridSize; ++j) {
				if (!gridInfo [i] [j].IsControlled)
					return false;
			}
		}
		return true;
	}

	public GridInfo GetLinkedGrid( UIElement ele )
	{
		if (gridInfo.Count < gridSize)
			InitGrid ();
		
		for (int i = 0; i < gridSize; ++i) {
			for (int j = 0; j < gridSize; ++j) { 
				if (gridInfo [i] [j].linkEle == ele)
					return gridInfo [i] [j];
			}
		}

		return null;
	}

	protected override void MAwake ()
	{
		base.MAwake ();

		InitGrid ();
		InitButton ();
	}

	public void InitButton()
	{
		buttonPrefabList = new List<PrefabInfo> ();
		string[] paths = {"UI/SystemButton",
			"UI/CreateButton",
		};

		foreach (var p in paths) {
			var buttons = Resources.LoadAll<GameObject> (p);
			foreach (var btn in buttons )
			{
				var info = new PrefabInfo ();
				info.prefab = btn;
				info.type = btn.GetComponent<UIButton> ().m_type;
				buttonPrefabList.Add (info);
			}
		}
	}

	public void InitGrid()
	{
		gridArray.Clear ();
		if (NineGridRoot != null || gridArray.Count < 9 ) {
			gridArray.AddRange (NineGridRoot.GetComponentsInChildren<RectTransform> ());
			gridArray.RemoveAt (0);
		}

		gridInfo.Clear ();
		for (int i = 0; i < gridSize; ++i) {
			var list = new List<GridInfo> ();
			for (int j = 0; j < gridSize; ++j) {
				var info = new GridInfo ();
				info.x = i;
				info.y = j;
				info.trans = GetGirdNine (i, j);
				list.Add (info);
			}
			gridInfo.Add (list);
		}
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		temScore = Mathf.Lerp (temScore, WorldManager.Instance.Score + 0.5f, 10f * Time.deltaTime);
		scoreText.text = ((int)temScore).ToString ();
	}

	public void MoveFrom( UIElement target , int fromX , int fromY , float duration = -1f )
	{
		if (target != null) {
			Vector3 fromPos = GetGirdNine (fromX, fromY).transform.position;
			target.PlayMoveFrom (fromPos, duration);
		}
	}

	public void MoveFrom( UIElement fromEle , UIElement target , float duration = -1f )
	{
		if (fromEle != null && target != null) {
			target.PlayMoveFrom (fromEle.transform.position, duration);
		}
	}

	public GameObject GetButtonPrefab( ButtonType type )
	{
		foreach (var p in buttonPrefabList) {
			if (p.type == type)
				return p.prefab;
		}
		if (buttonPrefabList.Count > 0)
			return buttonPrefabList [0].prefab;

		return null;
	}

	public UIButton AddButtonAtRandomPosition( ButtonType type , IconType iconType = IconType.None  )
	{
		int i = 0;
		int j = 0;
		while ( !IsGridFull() ) {
			i = Random.Range (0, gridSize);
			j = Random.Range (0, gridSize);
			if (IsGridAvaiable (i, j)) {
				break;
			}
		}

		if ( !IsGridFull() )
			return AddButton (type,  i, j , iconType);

		return null;
	}

	public UIButton AddButton( ButtonType type , int posX = 1 , int posY = 1, IconType iconType = IconType.None )
	{		
		if ( !IsGridAvaiable( posX , posY ) )
			return null ;
		var prefab = GetButtonPrefab (type);
		if (prefab != null) {
			var obj = Instantiate ( prefab , Root) as GameObject;
//		obj.GetComponent<RectTransform>().anchoredPosition3D = GetGrid (posX, posY).anchoredPosition3D;
			var button = obj.GetComponent<UILayerButton> ();
			if (button != null) {
				AddUIElement (button, posX, posY);
				button.Init (type, iconType);
//			temUIElement.Add (button);
			}

			return button;
		}
		return null;
	}

	/// <summary>
	/// Adds the user interface element.
	/// </summary>
	/// <returns><c>true</c>, if the block is empty and user interface element was added, <c>false</c> otherwise.</returns>
	/// <param name="ele">Ele.</param>
	/// <param name="posX">Position x.</param>
	/// <param name="posY">Position y.</param>
	public bool AddUIElement( UIElement ele , int posX , int posY )
	{
		var info = GetGridInfo (posX, posY);
		if (!info.IsControlled) {
			temUIElement.Add (ele);
			LinkGridInfo ( ele, posX, posY);
			ele.GetComponent<RectTransform> ().anchoredPosition = GetGirdNine (posX, posY).anchoredPosition;

			return true;
		}

		ele.OnExit ();
		return false;

	}

	public void RemoveElement( UIElement e )
	{
		var info = GetLinkedGrid (e);
		if (info != null)
			info.CleanEle ();
		e.OnExit ( true );
	}

	public void ClearElement()
	{
		foreach (var e in temUIElement) {
			RemoveElement (e);
		}

		temUIElement.Clear ();
	}

	public void ClearElementExcept( UIElement ele )
	{
		for (int i = temUIElement.Count - 1; i >= 0; i--) {
			if (temUIElement [i] != ele) {
				RemoveElement (temUIElement [i]);
				temUIElement.RemoveAt (i);
			}
		}
	}

	public List<UIElement> GetElements()
	{
		return temUIElement;
	}

	public int GetElementCount()
	{
		return temUIElement.Count;
	}
}
