
	public class TestPanelBase : BasePanel
	{


		public void InitBind()
			{
				UnityEngine.GameObject go = gameObject;
				_BtnTest =go.transform.Find("_BtnTest").gameObject.GetComponent<UnityEngine.UI.Button>();
				_BtnTest.onClick.AddListener(OnClickBtnTest);
				if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
					OnInitAndroid();
				if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer)
					OnInitIos();
				 _InitState = true;
			}

		[UnityEngine.HideInInspector]
		public bool _InitState = false;
		public System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, object>>> _TempCache = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<int, System.Collections.Generic.Dictionary<string, object>>>();
		[UnityEngine.HideInInspector]
		public UnityEngine.UI.Button _BtnTest;

		public override void OnExit(bool isDestroy)
			{
				base.OnExit(isDestroy);
				if (isDestroy)
				{
					 _InitState = false;
					if (_TempCache.Count>0)
						{
							for(int i=0;i<_TempCache.Count;i++)
								{
									_TempCache[i].Clear();
								}
						}
				}
				_TempCache.Clear();
				_BtnTest = null;
			}

		public virtual void OnInitAndroid()
			{
			}

		public virtual void OnInitIos()
			{
			}

		public virtual void OnClickBtnTest()
			{
				UnityEngine.Debug.LogError("[UIPrefab]TestPanel:OnClickBtnTest not implemented.");
			}

	}
