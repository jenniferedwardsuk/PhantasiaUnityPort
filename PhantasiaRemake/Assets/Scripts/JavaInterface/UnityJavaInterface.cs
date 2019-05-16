using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

public class UnityJavaInterface
{
    //todo: on connection stop, destroy javacanvas children and popups

    [SerializeField] HUDController networkHUD;

    UnityPlayerUIController UnityUIController;
    UnityPlayerController UnityPlayer;
    public pClient JavaClient;

    //public Frame f;
    public statusPne status;
    public buttonPne buttons;
    public msgPne messages;
    public userPne users;
    public chatPne chat;
    public compassPne compass;
    public errorDlog errorDialog;

    public UnityJavaInterface()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Warning: attempt to create duplicate instance of singleton " + this);
            //Destroy(this);
        }
    }
    static UnityJavaInterface Instance;
    internal static UnityJavaInterface GetInstance()
    {
        if (Instance == null)
        {
            Instance = new UnityJavaInterface();
        }
        if (Instance == null)
        {
            Debug.LogError("Could not create javainterface instance");
        }
        return Instance;
    }

    public void StartClient(UnityPlayerUIController UIlink)
    {
        UnityUIController = UIlink;

        JavaClient = new pClient();

        //event listeners
        status = JavaClient.status;
        buttons = JavaClient.buttons;
        messages = JavaClient.messages;
        users = JavaClient.users;
        chat = JavaClient.chat;
        compass = JavaClient.compass;
        errorDialog = JavaClient.errorDialog;

        if (!mainCanvas)
            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        
    }

    public void RefreshUI()
    {
        //Thread.Sleep(1000); //wait for UI to generate before resizing   //currently unnecessary

        RefreshAllLayouts(JavaClient.status);
        RefreshAllLayouts(JavaClient.messages);
        RefreshAllLayouts(JavaClient.buttons);
        RefreshAllLayouts(JavaClient.chat);
        RefreshAllLayouts(JavaClient.rightPane);

        RetrieveAllWanderingComponents(mainCanvas); //todo: doesn't work - too soon?
        GameObject[] popups = GameObject.FindGameObjectsWithTag("PopupCanvas");
        foreach (GameObject obj in popups)
        {
            Canvas canvas = obj.GetComponent<Canvas>();
            if (canvas && canvas.sortingOrder > -1) //exclude template
            {
                RetrieveAllWanderingComponents(obj);
            }
        }
    }

    public static string clientVersion = "1004";
    internal static string GetClientVersion()
    {
        return clientVersion;
    }

    internal static string GetNumParam()
    {
        return UnityPlayerController.GetInstance().ParamNum;   
        
    }

    internal static string GetHashParam()
    {
        return UnityPlayerController.GetInstance().ParamHash;
    }

    internal static string GetTimeParam()
    {
        //expects current time in ms - checks whether client packet is old when received at server
        int time = CFUNCTIONS.GetUnixEpoch(DateTime.Now);
        string str = time.ToString();
        return str;
    }

    #region socket management
    internal static void OpenSocket()
    {
        socketOpen = true;
    }

    internal static void CloseSocket()
    {
        socketOpen = false;
    }
    static bool socketOpen = false;

    internal void SendToSocket(byte[] bytes)
    {
        //in caller: byte[] bytes = Encoding.ASCII.GetBytes(theString);
        //UnityPlayer.CmdSendDataToServer(bytes);  //Cmd method has to happen on main thread
        if (!UnityPlayer)
        {
            UnityPlayer = UnityPlayerController.GetInstance();
            //if (UnityPlayer)
            //    UnityUIController = UnityPlayer.gameObject.GetComponent<UnityPlayerUIController>();   //can't call from thread. unnecessary sanity check anyway
            //if (!UnityUIController)
            //    Debug.LogError("Java client: Could not find latest player UI controller");
        }
        if (UnityPlayer)
        {
            Debug.Log("Java client: sending data to server");
            UnityPlayer.pendingData.Add(bytes);
        }
        else
        {
            Debug.LogError("Java client: Could not find player for socket data send");
        }
    }

    static byte[] waitingData;
    public static byte[] WaitingData
    {
        get
        {
            return waitingData;
        }

        set
        {
            if (socketOpen)
                waitingData = value;
        }
    }

    static byte[] bytesBuffer = new byte[] { };
    internal static string ReadFromSocket()
    {
        //byte[] bytes = null;
        int count = 0;

        //Debug.Log("bytesBuffer length is " + bytesBuffer.Length);

        while (bytesBuffer.Length == 0 && WaitingData == null)  //wait for a message to arrive
        {
            count++;
            if (count > 500000000)
            {
                count = 0;
                string msg = "Java client: player waiting for socket read...";
                Debug.Log(msg);
            }

            if (UnityGameController.StopApplication) //time to quit  //todo: share between client and server
            {
                Debug.Log("Java client: thread " + System.Threading.Thread.CurrentThread.Name + " stopping in ReadFromSocket");
                WaitingData = new byte[] { };
                break;
            }
            else
            {
                System.Threading.Thread.Sleep(33); //30fps
            }
            //break; //this line for java coroutine; loop is managed from top
            //WaitingData = Encoding.ASCII.GetBytes("-1"); //this line for java coroutine; loop is managed from top
        }
        if (WaitingData != null) //new data ready to add //todo: should WaitingData be a list / stack pending messages?
        {
            string bufferString = Encoding.ASCII.GetString(bytesBuffer);
            string waitingString = Encoding.ASCII.GetString(WaitingData);
            string addedData = bufferString + waitingString;
            bytesBuffer = Encoding.ASCII.GetBytes(addedData);
            WaitingData = null;
        }

        string someString = null;
        if (bytesBuffer != null)
        {

            //collect first string
            string bufferString = Encoding.ASCII.GetString(bytesBuffer);

            string stringFiltered = bufferString.Replace('\0', '£');
            //Debug.Log("Java client: thread " + System.Threading.Thread.CurrentThread.Name + " buffer before string collect: || " + stringFiltered + " ||");

            int firstNull = bufferString.IndexOf('\0');
            someString = bufferString.Substring(0, firstNull);

            //reduce buffer accordingly
            bufferString = bufferString.Substring(firstNull + 1);
            bytesBuffer = Encoding.ASCII.GetBytes(bufferString);

            stringFiltered = bufferString.Replace('\0', '£');
            //Debug.Log("Java client: thread " + System.Threading.Thread.CurrentThread.Name + " buffer after collect: || " + stringFiltered + " ||");

        }



        if (!UnityGameController.StopApplication)
        {
            string someStringFiltered = someString.Replace('\0', '£');
            //Debug.Log("Java client: thread " + System.Threading.Thread.CurrentThread.Name + " collected message: || " + someStringFiltered + " ||");
        }
        
        return someString;
    }
    internal static lThread lthread;
    public static bool startThread = false;
    public static bool stopThread = false;
    internal static Thread javathread;
    internal static void StartThread(JavaThread javaThread)
    {
        lthread = (lThread)javaThread; //only lThreads instantiate javathreads in phantasia
        //startThread = true; // defers to unityUIcontroller for coroutine
        Action startMethod = lthread.run;
        if (javathread == null)
        {
            javathread = new Thread(new ThreadStart(startMethod));

            UnityPlayerController player = UnityPlayerController.GetInstance();
            if (player)
            {
                short networkPlayerID = UnityPlayerController.GetInstance().playerControllerId;
                javathread.Name = networkPlayerID.ToString();
                javathread.Start();
                Debug.Log("created new Java thread with ID " + javathread.Name);
            }
            else
            {
                Debug.LogError("Could not find player for new Java thread");
            }
        }
        else
        {
            Debug.LogError("Tried to create java thread when one is already referenced");
        }

    }
    internal static void StopThread(JavaThread javaThread)
    {
        //stopThread = true; // defers to unityUIcontroller for coroutine

        lthread = (lThread)javaThread;
        //javathread.Interrupt();
        //javathread.Abort();
        // relying on java thread stopping itself

        //todo: destroy popups

        // disconnect player from networkserver, so that they can reconnect if they want
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(DisconnectClient);
    }
    internal static void DisconnectClient()
    {
        if (HUDController.GetInstance())
            HUDController.GetInstance().Disconnect();
    }
    //internal IEnumerator RunThread(lThread thread)
    //{
    //    //thread.run();
    //    yield return null;
    //}
    #endregion socket management


    internal static int maxSortOrder;
    internal static void updateMaxSortOrder(int sortingOrder)
    {
        maxSortOrder = Mathf.Min(99, sortingOrder);
        //todo: roll under each canvas if 99 is hit
    }

    internal static Font GetFontArial() //unused. not working anyway?
    {
        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    internal static Font GetFontTimesRoman()
    {
        return (Font)Resources.Load("TIMESBD");
    }

    internal static Font GetFontHelvetica()
    {
        return (Font)Resources.Load("Helvetica");
    }

    #region main canvas
    internal static void setMainTitle()
    {
        //todo      //throw new NotImplementedException();
    }

    internal static void setMainCanvasSize(int v1, int v2)
    {
        if (mainCanvas && mainCanvas.GetComponent<RectTransform>())
        {
            //mainCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(v1,v2); //todo ? no can do on main canvas
        }
        else
        {
            Debug.LogError("Main canvas not ready for resize");
        }
    }

    internal static void setMainCanvasBackground(JavaColor backgroundColor)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_setMainCanvasBackground, backgroundColor);
    }
    internal static void M_setMainCanvasBackground(JavaColor backgroundColor)
    {
        if (mainCanvas && mainCanvas.GetComponent<Image>())
        {
            mainCanvas.GetComponent<Image>().color = backgroundColor.GetUnityColor();
        }
        else
        {
            Debug.Log("Main canvas not ready for image color");
        }
    }

    internal static void HideMainCanvas()
    {
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Error: Main canvas not ready for hiding");
        }
    }
    internal static void ShowMainCanvas()
    {
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Error: Main canvas not ready for showing");
        }
    }
    #endregion main canvas

    internal static Sprite GetImage(JavaURL uRL, string filename)
    {
        Sprite img = Resources.Load(filename.Substring(0, filename.Length - 4), typeof(Sprite)) as Sprite; //Resources.Load<Sprite>(filename);
        //Debug.Log("trying load " + filename.Substring(0, filename.Length - 4) + " img is " + img);

        return img;
    }

    #region layout control
    internal static void PackPopupLayout(JavaComponent sourceComponent)
    {
        //The default layout for a dialog is BorderLayout
        UnityPopupComponents popupComponents = (UnityPopupComponents)sourceComponent.unityComponentGroup;
        CreateOrRefreshBorderLayout(sourceComponent, true);

        //todo - packing. expand children to the minimum size for their content, then resize popup panel accordingly

        GameObject panel = popupComponents.panelComponent.gameObject;
        RectTransform parent = panel.GetComponent<RectTransform>();
        if (!panel.GetComponent<GridLayoutGroup>() && !panel.GetComponent<UnityBorderLayoutManager>() && panel.name.Contains("Panel"))
        {
            GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>(); //this is an interim solution
            grid.cellSize = new Vector2(
                panel.GetComponent<RectTransform>().sizeDelta.x / Mathf.Max(panel.transform.childCount, 1),
                panel.GetComponent<RectTransform>().sizeDelta.y / Mathf.Max(panel.transform.childCount, 1)
                );
        }

        int componentsInPanel = parent.childCount;
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            if (!child.GetComponent<GridLayoutGroup>() && !child.GetComponent<UnityBorderLayoutManager>() && child.name.Contains("Panel"))
            {
                GridLayoutGroup grid = child.AddComponent<GridLayoutGroup>();
                grid.cellSize = new Vector2(
                    child.GetComponent<RectTransform>().sizeDelta.x / Mathf.Max(child.transform.childCount, 1),
                    child.GetComponent<RectTransform>().sizeDelta.y / Mathf.Max(child.transform.childCount, 1)
                    );
            }

            for (int j = 0; j < child.transform.childCount; j++)
            {
                GameObject grandchild = child.transform.GetChild(j).gameObject;
                if (!grandchild.GetComponent<GridLayoutGroup>() && !grandchild.GetComponent<UnityBorderLayoutManager>() && grandchild.name.Contains("Panel"))
                {
                    GridLayoutGroup grid = grandchild.AddComponent<GridLayoutGroup>();
                    //grid.cellSize = new Vector2(100, 50);
                    grid.cellSize = new Vector2(
                        grandchild.GetComponent<RectTransform>().sizeDelta.x / Mathf.Max(grandchild.transform.childCount, 1),
                        grandchild.GetComponent<RectTransform>().sizeDelta.y / Mathf.Max(grandchild.transform.childCount, 1)
                        );
                }
            }

            //RectTransform child = parent.GetChild(i).gameObject.GetComponent<RectTransform>();
            //child.sizeDelta = new Vector2(parent.sizeDelta.x / componentsInPanel, parent.sizeDelta.y / componentsInPanel);
            //child.position = parent.position;
        }
    }

    public static void RetrieveAllWanderingComponents(GameObject target)
    {
        RectTransform targetRect = target.GetComponent<RectTransform>();
        if (targetRect)
        {
            for (int i = 0; i < target.transform.childCount; i++)
            {
                RectTransform childRect = target.transform.GetChild(i).GetComponent<RectTransform>();
                if (childRect)
                {
                    // starts bottom left and rotates to top left, then top right, and finally bottom right
                    Vector3[] targetCorners = new Vector3[4];
                    targetRect.GetWorldCorners(targetCorners);
                    Vector3[] childCorners = new Vector3[4];
                    childRect.GetWorldCorners(childCorners);

                    if (childCorners[0].x < targetCorners[0].x - 10 || childCorners[0].y < targetCorners[0].y - 10
                        || childCorners[2].x > targetCorners[2].x + 10 || childCorners[2].y > targetCorners[2].y + 10)
                    {
                        childRect.position = targetRect.position;
                    }

                    if (childRect.sizeDelta.x > targetRect.sizeDelta.x)
                    {
                        childRect.sizeDelta = new Vector2(targetRect.sizeDelta.x, childRect.sizeDelta.y);
                    }
                    if (childRect.sizeDelta.y > targetRect.sizeDelta.y)
                    {
                        childRect.sizeDelta = new Vector2(childRect.sizeDelta.x, targetRect.sizeDelta.y);
                    }
                }

                if (!target.GetComponent<ScrollRect>())
                    RetrieveAllWanderingComponents(target.transform.GetChild(i).gameObject);
            }
        }
    }

    internal static void RefreshAllLayouts(JavaComponent sourceComponent)
    {
        JavaLayout layout = sourceComponent.getLayout();
        if (layout != null)
        {
            if (layout.GetType() == typeof(BorderLayout))
            {
                CreateOrRefreshBorderLayout(sourceComponent);
            }
            else if (layout.GetType() == typeof(GridBagLayout))
            {
                CreateOrRefreshGridBagLayout(sourceComponent);
            }
            else if (layout.GetType() == typeof(FlowLayout))
            {
                //unity controlled
                //refresh layout not needed, no flowlayouts have children in phantasia
            }
            else if (layout.GetType() == typeof(JavaGridLayout))
            {
                //Debug.Log("updating grid layout on " + sourceComponent.unityComponentGroup.rectComponent.gameObject.name);
                JavaGridLayout grid = (JavaGridLayout)layout;
                CreateOrRefreshGridLayout(sourceComponent, grid);
                LayoutRebuilder.ForceRebuildLayoutImmediate(sourceComponent.unityComponentGroup.rectComponent); //refresh layout to allow grid children to resize
            }
        }
        else
        {
            //todo - no layout rule on obj, so retrieve any stray children - and lay them out? (vertical layout group?)
            RetrieveAllWanderingComponents(sourceComponent.unityComponentGroup.rectComponent.gameObject);
        }

        foreach (JavaComponent comp in sourceComponent.childComponents)
        {
            //refresh children
            RefreshAllLayouts(comp);

            //check for subcanvases
            GameObject compObj = comp.unityComponentGroup.rectComponent.gameObject;
            for (int i = 0; i < compObj.transform.childCount; i++)
            {
                GameObject child = compObj.transform.GetChild(i).gameObject;
                if (child.name.Contains("GenLabel") 
                    || child.name.Contains("GenText") 
                    || child.name.Contains("GenButton")
                    //|| child.name.Contains("Scroll")
                    )
                {
                    RectTransform childRect = child.GetComponent<RectTransform>();
                    RectTransform parentRect = compObj.GetComponent<RectTransform>();
                    childRect.sizeDelta = parentRect.sizeDelta;
                    Vector3[] parentPanelCorners = new Vector3[4];
                    parentRect.GetWorldCorners(parentPanelCorners);
                    childRect.position = parentPanelCorners[1] + new Vector3(childRect.sizeDelta.x / 2, -1 * childRect.sizeDelta.y / 2, 0);
                }
            }
        }
    }

    internal static void SetLayout(JavaComponent sourceComponent, UnityComponentGroup unityPanel, JavaLayout gridLayout)
    {
        if (gridLayout.GetType() == typeof(BorderLayout))
        {
            //GridLayoutGroup group = unityPanel.rectComponent.gameObject.AddComponent<GridLayoutGroup>();
            //group.spacing = new Vector2(((BorderLayout)gridLayout).XGap, ((BorderLayout)gridLayout).YGap);
            //todo
            CreateOrRefreshBorderLayout(sourceComponent);
        }
        else if (gridLayout.GetType() == typeof(GridBagLayout))
        {
            CreateOrRefreshGridBagLayout(sourceComponent);
        }
        else if (gridLayout.GetType() == typeof(FlowLayout))
        {
            HorizontalLayoutGroup horizLayout = unityPanel.rectComponent.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizLayout.childControlHeight = false;
            horizLayout.childControlWidth = false;
            horizLayout.childAlignment = TextAnchor.MiddleCenter;
            horizLayout.childForceExpandHeight = false;
            horizLayout.childForceExpandWidth = false;
            horizLayout.padding.left = ((FlowLayout)gridLayout).Hgap;
            horizLayout.padding.right = ((FlowLayout)gridLayout).Hgap;
            horizLayout.padding.top = ((FlowLayout)gridLayout).Vgap;
            horizLayout.padding.bottom = ((FlowLayout)gridLayout).Vgap;
            horizLayout.spacing = Mathf.Max(((FlowLayout)gridLayout).Vgap, ((FlowLayout)gridLayout).Hgap);
        }
        else if (gridLayout.GetType() == typeof(JavaGridLayout))
        {
            JavaGridLayout grid = (JavaGridLayout)gridLayout;
            CreateOrRefreshGridLayout(sourceComponent, grid);
        }
    }

    private static void CreateOrRefreshGridLayout(JavaComponent sourceComponent, JavaGridLayout grid)
    {
        if (grid != null)
        {
            RectTransform componentUnityRect = sourceComponent.unityComponentGroup.rectComponent;

            GridLayoutGroup group = componentUnityRect.gameObject.GetComponent<GridLayoutGroup>();
            if (!group)
                group = componentUnityRect.gameObject.AddComponent<GridLayoutGroup>();

            group.spacing = new Vector2(grid.hgap, grid.vgap);
            group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            group.constraintCount = grid.cols;
            group.childAlignment = TextAnchor.MiddleCenter;
            group.cellSize = new Vector2(
                componentUnityRect.sizeDelta.x / (grid.cols),// + grid.hgap * grid.cols),
                componentUnityRect.sizeDelta.y / (grid.rows));// + grid.vgap * grid.rows));
        }
    }

    internal static void CreateOrRefreshBorderLayout(JavaComponent component, bool isPopup = false)
    {
        if (component.unityComponentGroup != null && component.unityComponentGroup.rectComponent)
        {
            GameObject componentUnityObj = component.unityComponentGroup.rectComponent.gameObject;

            if (isPopup)
            {
                Transform popupPanel = component.unityComponentGroup.rectComponent.Find("Panel");
                if (popupPanel)
                    componentUnityObj = popupPanel.gameObject;
            }

            UnityBorderLayoutManager unityLayoutManager = componentUnityObj.GetComponent<UnityBorderLayoutManager>();
            if (!unityLayoutManager)
            {
                //Debug.Log("creating border layout");
                unityLayoutManager = componentUnityObj.AddComponent<UnityBorderLayoutManager>();
            }
            else
            {
                //Debug.Log("refreshing border layout");
            }

            if (unityLayoutManager)
            {
                unityLayoutManager.components = component.childComponents;
                unityLayoutManager.sourceComponent = component;
                unityLayoutManager.UpdateLayout(isPopup);
            }
        }
    }

    internal static void CreateOrRefreshGridBagLayout(JavaComponent component)
    {
        GameObject componentUnityObj = component.unityComponentGroup.rectComponent.gameObject;
        UnityGridBagLayoutManager unityLayoutManager = componentUnityObj.GetComponent<UnityGridBagLayoutManager>();
        if (!unityLayoutManager)
        {
            unityLayoutManager = componentUnityObj.AddComponent<UnityGridBagLayoutManager>();
        }

        if (unityLayoutManager)
        {
            unityLayoutManager.components = component.childComponents;
            unityLayoutManager.sourceComponent = component;
            unityLayoutManager.UpdateLayout();
        }
    }
    #endregion layout control
    

    #region events
    public void KeyPressed(KeyCode key)
    {
        //used by pClient and chatPne - but chatPne throws to pClient, so ignoring
        KeyEvent evt = new KeyEvent();
        int JavaKey = 0;
        switch (key)
        {
            case KeyCode.F1:
                JavaKey = KeyEvent.VK_F1;
                break;
            case KeyCode.F2:
                JavaKey = KeyEvent.VK_F2;
                break;
            case KeyCode.F3:
                JavaKey = KeyEvent.VK_F3;
                break;
            case KeyCode.F4:
                JavaKey = KeyEvent.VK_F4;
                break;
            case KeyCode.F5:
                JavaKey = KeyEvent.VK_F5;
                break;
            case KeyCode.F6:
                JavaKey = KeyEvent.VK_F6;
                break;
            case KeyCode.F7:
                JavaKey = KeyEvent.VK_F7;
                break;
            case KeyCode.F8:
                JavaKey = KeyEvent.VK_F8;
                break;
            case KeyCode.Alpha1:
                JavaKey = KeyEvent.VK_1;
                break;
            case KeyCode.Alpha2:
                JavaKey = KeyEvent.VK_2;
                break;
            case KeyCode.Alpha3:
                JavaKey = KeyEvent.VK_3;
                break;
            case KeyCode.Alpha4:
                JavaKey = KeyEvent.VK_4;
                break;
            case KeyCode.Alpha5:
                JavaKey = KeyEvent.VK_5;
                break;
            case KeyCode.Alpha6:
                JavaKey = KeyEvent.VK_6;
                break;
            case KeyCode.Alpha7:
                JavaKey = KeyEvent.VK_7;
                break;
            case KeyCode.Alpha8:
                JavaKey = KeyEvent.VK_8;
                break;
            case KeyCode.Keypad1:
                JavaKey = KeyEvent.VK_NUMPAD1;
                break;
            case KeyCode.Keypad2:
                JavaKey = KeyEvent.VK_NUMPAD2;
                break;
            case KeyCode.Keypad3:
                JavaKey = KeyEvent.VK_NUMPAD3;
                break;
            case KeyCode.Keypad4:
                JavaKey = KeyEvent.VK_NUMPAD4;
                break;
            case KeyCode.Keypad5:
                JavaKey = KeyEvent.VK_NUMPAD5;
                break;
            case KeyCode.Keypad6:
                JavaKey = KeyEvent.VK_NUMPAD6;
                break;
            case KeyCode.Keypad7:
                JavaKey = KeyEvent.VK_NUMPAD7;
                break;
            case KeyCode.Keypad8:
                JavaKey = KeyEvent.VK_NUMPAD8;
                break;
            case KeyCode.Keypad9:
                JavaKey = KeyEvent.VK_NUMPAD9;
                break;
            case KeyCode.Space:
                JavaKey = KeyEvent.VK_SPACE;
                break;
            case KeyCode.Return:
                JavaKey = KeyEvent.VK_RETURN;
                break;
        }
        if (JavaKey > 0)
        {
            Debug.Log("passing keycode to Java");
            evt.setCurrentKeyCode(JavaKey);
            JavaClient.keyPressed(evt);
        }
    }
    internal static UnityKeyListener AddKeyListener()
    {
        //not required / handled universally
        return null;
    }

    public void ActionButton_Click(int buttonNum)
    {
        ActionEvent evt = GetActionButtonEvent(buttonNum);
        chat.actionPerformed(evt);
    }
    public void CompassButton_Click(int buttonNum)
    {
        ActionEvent evt = GetCompassButtonEvent(buttonNum);
        chat.actionPerformed(evt);
    }
    public void ChatButton_Click(int buttonNum)
    {
        ActionEvent evt = GetChatButtonEvent(buttonNum);
        chat.actionPerformed(evt);
    }
    public void DialogButton_Click(int buttonNum)
    {
        ActionEvent evt = GetDialogButtonEvent(buttonNum);
        errorDialog.actionPerformed(evt);
    }

    ActionEvent GetChatButtonEvent(int buttonNum)
    {
        ActionEvent evt = null;
        if (buttonNum == 1) //send
        {
            evt = chat.clearJavaButton.getActionEvent();
        }
        else //clear
        {
            evt = chat.sendJavaButton.getActionEvent();
        }
        return evt;
    }
    ActionEvent GetDialogButtonEvent(int buttonNum)
    {
        ActionEvent evt = null;
        evt = errorDialog.theJavaButton.getActionEvent();
        return evt;
    }
    ActionEvent GetCompassButtonEvent(int buttonNum)
    {
        ActionEvent evt = null;
        evt = compass.theJavaButtons[buttonNum - 1].getActionEvent();
        return evt;
    }
    ActionEvent GetActionButtonEvent(int buttonNum)
    {
        ActionEvent evt = null;
        evt = buttons.theJavaButtons[buttonNum - 1].getActionEvent();
        return evt;
    }
    #endregion events
    #region components
    static GameObject latestPanel; //add new components as children of this panel
    static GameObject latestParentPanel;
    static GameObject latestParentParentPanel;
    public static GameObject mainCanvas;
    static bool writeDebug = true;
    static int debugCount = 0;
    
    internal static void AddPopup(Dialog source)
    {
        UnityJavaUIFuncQueue.GetInstance().QueueUIMethod(M_AddPopup, source);
    }
    internal static void M_AddPopup(Dialog source)
    {
        GameObject popupTemplateCanvas = GameObject.FindGameObjectWithTag("PopupCanvas");
        if (popupTemplateCanvas)
        {
            GameObject popupCanvas = GameObject.Instantiate(popupTemplateCanvas);
            popupCanvas.GetComponent<Canvas>().sortingOrder = 3; //todo: arbitrary
            latestPanel.transform.parent = popupCanvas.transform;

            UnityPopupComponents popupComponents = new UnityPopupComponents();

            popupComponents.rectComponent = popupCanvas.GetComponent<RectTransform>();
            popupComponents.rectComponent.anchorMin = new Vector2(0, 1); //top left
            popupComponents.rectComponent.anchorMax = new Vector2(0, 1); //top left

            GameObject BGPanel = popupCanvas.transform.Find("BGPanel").gameObject;
            GameObject titleTextObj = BGPanel.transform.Find("TitleText").gameObject;
            GameObject mainPanel = popupCanvas.transform.Find("Panel").gameObject;

            popupComponents.titleTextComponent = titleTextObj.GetComponent<Text>();
            popupComponents.panelComponent = mainPanel;
            popupComponents.bgImageComponent = mainPanel.GetComponent<Image>();
            popupComponents.canvasComponent = popupCanvas.GetComponent<Canvas>();

            //return popupComponents;
            source.SetComponentGroup(popupComponents);
        }
        else
        {
            Debug.LogError("Template not found for popup component");
        }
        //return null;
    }
    public static void AddPanel(string name, bool isCanvasParent, JavaComponent source)
    {
        if (name.Contains("Frame"))
        {
            UnityPanelComponents panelComponents = new UnityPanelComponents();
            if (!mainCanvas)
                mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
            panelComponents.rectComponent = mainCanvas.GetComponent<RectTransform>();
            panelComponents.imageComponent = mainCanvas.GetComponent<Image>();
            
            //return panelComponents;
            source.SetComponentGroup(panelComponents);
        }
        else
        {
            //return AddPanel(name, isCanvasParent, false, false);
            AddPanel(name, isCanvasParent, false, false, source);
        }
    }
    public static void AddPanel(string name, bool isCanvasParent, bool isMiddleParent, bool parentToPreviousLevel, JavaComponent source)
    {
        mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        mainCanvas.GetComponent<Image>().color = new Color(1,1,1,1);
        GameObject Panel = new GameObject("GenPanel" + name);
        UnityPanelComponents panelComponents = new UnityPanelComponents();
        panelComponents.rectComponent = Panel.AddComponent<RectTransform>();
        panelComponents.rectComponent.anchorMin = new Vector2(0, 1); //top left
        panelComponents.rectComponent.anchorMax = new Vector2(0, 1); //top left
        Panel.AddComponent<CanvasRenderer>();
        panelComponents.imageComponent = Panel.AddComponent<Image>();
        Color imgcolor = new Color(1, 1, 1, 1); //todo: only works sometimes
        panelComponents.imageComponent.color = imgcolor;

        if (name == "-CanvasDialog")
        {
            panelComponents.imageComponent.enabled = false;
        }

        //set parent
        if (isCanvasParent || !latestPanel) //first panel, or new top level
        {
            //Debug.Log("(no parent for this panel) " + Panel.name);
            Panel.transform.parent = mainCanvas.transform;
        }
        else if (latestParentPanel && parentToPreviousLevel) //move up one level
        {
            //Debug.Log("(parenting up one level) " + Panel.name);
            Panel.transform.parent = latestParentPanel.transform;
        }
        else //parent to previous panel
        {
            //Debug.Log("(parenting to latest panel) " + Panel.name);
            Panel.transform.parent = latestPanel.transform;
        }

        //set latest panels
        if (isCanvasParent || !latestPanel)
        {
            latestPanel = Panel;
        }
        else if (isMiddleParent)
        {
            if (!parentToPreviousLevel)
                latestParentPanel = latestPanel;
            latestPanel = Panel;
            if (writeDebug)
            {
                //Debug.Log("latest parent panel is " + latestParentPanel.name);
                //Debug.Log("latest panel is " + latestPanel.name);
                //Debug.Log("this panel is " + Panel.name);
                debugCount++;
                if (debugCount > 5)
                    writeDebug = false;
            }
        }

        panelComponents.rectComponent.sizeDelta = new Vector2(200, 200); //temp //Panel.transform.parent.GetComponent<RectTransform>().sizeDelta; //temp
        Vector3[] parentPanelCorners = new Vector3[4];
        panelComponents.rectComponent.parent.GetComponent<RectTransform>().GetWorldCorners(parentPanelCorners);
        panelComponents.rectComponent.position = parentPanelCorners[1] + new Vector3(panelComponents.rectComponent.sizeDelta.x / 2, -1 * panelComponents.rectComponent.sizeDelta.y / 2, 0); //Panel.transform.parent.GetComponent<RectTransform>().position; //temp

        //return panelComponents;
        source.SetComponentGroup(panelComponents);
    }
    public static UnityTextComponents AddLabel(JavaComponent specificParent)
    {
        UnityTextComponents textComponents = null;
        if (latestPanel)
        {
            GameObject Label = new GameObject("GenLabel");
            textComponents = new UnityTextComponents();
            textComponents.rectComponent = Label.AddComponent<RectTransform>();
            textComponents.rectComponent.anchorMin = new Vector2(0, 1); //top left
            textComponents.rectComponent.anchorMax = new Vector2(0, 1); //top left
            Label.AddComponent<CanvasRenderer>();
            textComponents.textComponent = Label.AddComponent<Text>();
            textComponents.textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponents.textComponent.color = new Color(0,0,0,1);
            if (specificParent != null)
                Label.transform.parent = specificParent.unityComponentGroup.rectComponent.transform;
            else
                Label.transform.parent = latestPanel.transform;

            textComponents.rectComponent.sizeDelta = Label.transform.parent.GetComponent<RectTransform>().sizeDelta;
            Vector3[] parentPanelCorners = new Vector3[4];
            Label.transform.parent.GetComponent<RectTransform>().GetWorldCorners(parentPanelCorners);
            textComponents.rectComponent.position = parentPanelCorners[1] + new Vector3(textComponents.rectComponent.sizeDelta.x / 2, -1 * textComponents.rectComponent.sizeDelta.y / 2, 0);
        }
        else
        {
            Debug.LogError("Panel not found for label component");
        }
        return textComponents;
    }
    public static UnityButtonComponents AddButton()
    {
        UnityButtonComponents buttonComponents = null;
        if (latestPanel)
        {
            GameObject Button = new GameObject("GenButton");
            GameObject ButtonText = new GameObject("GenText");
            buttonComponents = new UnityButtonComponents();
            buttonComponents.rectComponent = Button.AddComponent<RectTransform>();
            buttonComponents.rectComponent.anchorMin = new Vector2(0, 1); //top left
            buttonComponents.rectComponent.anchorMax = new Vector2(0, 1); //top left
            Button.AddComponent<CanvasRenderer>();
            Image image = Button.AddComponent<Image>();
            //Sprite img = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"); //doesn't work in build
            Sprite img = Resources.Load("dummybutton", typeof(Sprite)) as Sprite;
            image.sprite = img;
            image.type = Image.Type.Sliced;
            buttonComponents.buttonComponent = Button.AddComponent<Button>();
            buttonComponents.buttonComponent.enabled = true; //todo - not working?
            buttonComponents.buttonComponent.targetGraphic = image;
            buttonComponents.textComponent = ButtonText.AddComponent<Text>();
            buttonComponents.textComponent.color = new Color(0, 0, 0, 1);
            buttonComponents.textComponent.alignment = TextAnchor.MiddleCenter;
            buttonComponents.textComponent.font = GetFontHelvetica();
            Button.transform.parent = latestPanel.transform;
            ButtonText.transform.parent = Button.transform;
        }
        else
        {
            Debug.LogError("Panel not found for button component");
        }
        return buttonComponents;
    }
    internal static UnityInputFieldComponents AddTextEntryField()
    {
        UnityInputFieldComponents inputfieldComponents = null;
        if (latestPanel)
        {
            GameObject inputTemplate = null;
            GameObject templateCanvas = GameObject.FindGameObjectWithTag("TemplateCanvas");
            for (int i = 0; i < templateCanvas.transform.childCount; i++)
            {
                if (templateCanvas.transform.GetChild(i).CompareTag("InputFieldTemplate"))
                {
                    inputTemplate = templateCanvas.transform.GetChild(i).gameObject;
                }
            }
            if (inputTemplate)
            {
                GameObject InputField = GameObject.Instantiate(inputTemplate);
                GameObject TextPlaceholder = InputField.transform.GetChild(0).gameObject;
                GameObject TextMain = InputField.transform.GetChild(1).gameObject;

                inputfieldComponents = new UnityInputFieldComponents();
                inputfieldComponents.rectComponent = InputField.GetComponent<RectTransform>();
                inputfieldComponents.rectComponent.anchorMin = new Vector2(0, 1); //top left
                inputfieldComponents.rectComponent.anchorMax = new Vector2(0, 1); //top left
                inputfieldComponents.inputFieldComponent = InputField.GetComponent<InputField>();
                inputfieldComponents.placeholderTextComponent = TextPlaceholder.GetComponent<Text>();
                inputfieldComponents.textComponent = TextMain.GetComponent<Text>();

                InputField.transform.parent = latestPanel.transform;
            }
            else
            {
                Debug.LogError("Template not found for inputfield component");
            }
        }
        else
        {
            Debug.LogError("Panel not found for inputfield component");
        }
        return inputfieldComponents;
    }
    public static UnityScrollComponents AddScrollArea()
    {
        UnityScrollComponents scrollComponents = null;
        if (latestPanel)
        {
            GameObject scrollTemplate = null;
            GameObject templateCanvas = GameObject.FindGameObjectWithTag("TemplateCanvas");
            for (int i = 0; i < templateCanvas.transform.childCount; i++)
            {
                if (templateCanvas.transform.GetChild(i).CompareTag("ScrollTemplate"))
                {
                    scrollTemplate = templateCanvas.transform.GetChild(i).gameObject;
                }

            }
            if (scrollTemplate)
            {
                GameObject ScrollArea = GameObject.Instantiate(scrollTemplate);
                GameObject ScrollContent = FindScrollContent(ScrollArea);

                scrollComponents = new UnityScrollComponents();
                scrollComponents.rectComponent = ScrollArea.GetComponent<RectTransform>();
                scrollComponents.rectComponent.anchorMin = new Vector2(0, 1); //top left
                scrollComponents.rectComponent.anchorMax = new Vector2(0, 1); //top left
                scrollComponents.scrollComponent = ScrollArea.GetComponent<ScrollRect>();
                scrollComponents.contentTextComponent = ScrollContent.GetComponentInChildren<Text>();
                if (!scrollComponents.contentTextComponent)
                {
                    scrollComponents.contentTextComponent = ScrollContent.AddComponent<Text>();
                    scrollComponents.contentTextComponent.color = new Color(0, 0, 0, 1);
                }
                ScrollArea.transform.parent = latestPanel.transform;
            }
            else
            {
                Debug.LogError("Template not found for scroll component");
            }
        }
        else
        {
            Debug.LogError("Panel not found for scroll component");
        }
        return scrollComponents;
    }
    public static UnityListComponents AddListScroll()
    {
        UnityListComponents listComponents = null;
        if (latestPanel)
        {
            GameObject listTemplate = null;
            GameObject templateCanvas = GameObject.FindGameObjectWithTag("TemplateCanvas");
            for (int i = 0; i < templateCanvas.transform.childCount; i++)
            {
                if (templateCanvas.transform.GetChild(i).CompareTag("ListTemplate"))
                {
                    listTemplate = templateCanvas.transform.GetChild(i).gameObject;
                }

            }
            if (listTemplate)
            {
                GameObject ScrollArea = GameObject.Instantiate(listTemplate);
                GameObject ScrollContent = FindScrollContent(ScrollArea);

                listComponents = new UnityListComponents();
                listComponents.rectComponent = ScrollArea.GetComponent<RectTransform>();
                listComponents.rectComponent.anchorMin = new Vector2(0, 1); //top left
                listComponents.rectComponent.anchorMax = new Vector2(0, 1); //top left
                listComponents.scrollComponent = ScrollArea.GetComponent<ScrollRect>();
                listComponents.contentListItems = new List<GameObject>();
                for (int i = 0; i < ScrollContent.transform.childCount; i++)
                {
                    listComponents.contentListItems.Add(ScrollContent.transform.GetChild(i).gameObject);
                }
                ScrollArea.transform.parent = latestPanel.transform;
            }
            else
            {
                Debug.LogError("Template not found for list scroll component");
            }
        }
        else
        {
            Debug.LogError("Panel not found for list scroll component");
        }
        return listComponents;
    }
    internal static void DrawLine(RectTransform sourceTransform, int x1, int y1, int x2, int y2, JavaColor contextColor)
    {
        if (sourceTransform)
        {
            GameObject lineChild = GetGenChild("GenLine", sourceTransform);
            if (!lineChild)
            {
                GameObject drawTemplate = GetDrawTemplate();
                if (drawTemplate)
                {
                    lineChild = GameObject.Instantiate(drawTemplate);
                }
            }
            if (lineChild)
            {
                Vector3[] panelCorners = new Vector3[4];
                sourceTransform.GetWorldCorners(panelCorners);

                lineChild.name = "GenLine";
                lineChild.transform.parent = sourceTransform;
                lineChild.GetComponent<RectTransform>().position = panelCorners[1] + new Vector3(x1, y1 * -1, 0); //todo - placement seems odd on lines
                float lineLength = Mathf.Sqrt(Mathf.Pow((x2 - x1), 2) + Mathf.Pow((y2 - y1), 2));
                lineChild.GetComponent<RectTransform>().sizeDelta = new Vector2(lineLength, 2);
                //todo: rotate end to x2,y2
                lineChild.GetComponent<Image>().color = contextColor.GetUnityColor();
            }
            else
            {
                Debug.LogError("lineChild not created for drawline");
            }
        }
        else
        {
            Debug.LogError("Transform not found for drawline");
        }
    }
    internal static void DrawRect(RectTransform sourceTransform, int x, int y, int width, int height, JavaColor contextColor)
    {
        if (sourceTransform)
        {
            GameObject lineChild = GetGenChild("GenRect", sourceTransform);
            if (!lineChild)
            {
                GameObject drawTemplate = GetDrawTemplate();
                if (drawTemplate)
                {
                    lineChild = GameObject.Instantiate(drawTemplate);
                }
            }
            if (lineChild)
            {
                Vector3[] panelCorners = new Vector3[4];
                sourceTransform.GetWorldCorners(panelCorners);

                lineChild.name = "GenRect";
                lineChild.transform.parent = sourceTransform;
                lineChild.GetComponent<RectTransform>().position = panelCorners[1] + new Vector3(x, y * -1, 0); //todo - placement seems odd on lines
                lineChild.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                lineChild.GetComponent<Image>().color = contextColor.GetUnityColor();
            }
            else
            {
                Debug.LogError("lineChild not created for drawline");
            }
        }
        else
        {
            Debug.LogError("Transform not found for drawrect");
        }
    }
    private static GameObject GetGenChild(string childName, RectTransform sourceTransform)
    {
        GameObject obj = null;
        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            if(sourceTransform.GetChild(i).name.Equals(childName))
            {
                obj = sourceTransform.GetChild(i).gameObject;
            }
        }
        return obj;
    }
    internal static GameObject GetDrawTemplate()
    {
        GameObject drawTemplate = null;
        GameObject templateCanvas = GameObject.FindGameObjectWithTag("TemplateCanvas");
        for (int i = 0; i < templateCanvas.transform.childCount; i++)
        {
            if (templateCanvas.transform.GetChild(i).CompareTag("DrawTemplate"))
            {
                drawTemplate = templateCanvas.transform.GetChild(i).gameObject;
            }
        }
        if (!drawTemplate)
        {
            Debug.LogError("Template not found for drawline");
        }
        return drawTemplate;
    }
    public static GameObject FindScrollContent(GameObject ScrollViewObject)
    {
        RectTransform RetVal = null;
        Transform[] Temp = ScrollViewObject.GetComponentsInChildren<Transform>();
        foreach (Transform Child in Temp)
        {
            if (Child.name == "Content") { RetVal = Child.gameObject.GetComponent<RectTransform>(); }
        }
        return RetVal.gameObject;
    }
    #endregion components
}

public class UnityComponentGroup
{
    public RectTransform rectComponent;
}

public class UnityPopupComponents : UnityComponentGroup
{
    public Canvas canvasComponent;
    public GameObject panelComponent;
    public Image bgImageComponent;
    public Text titleTextComponent;
}
public class UnityButtonComponents : UnityComponentGroup
{
    public Text textComponent;
    public Button buttonComponent;
}
public class UnityScrollComponents : UnityComponentGroup
{
    public ScrollRect scrollComponent;
    public Text contentTextComponent;
}
public class UnityInputFieldComponents : UnityComponentGroup
{
    public InputField inputFieldComponent;
    public Text placeholderTextComponent;
    public Text textComponent;
}
public class UnityPanelComponents : UnityComponentGroup
{
    public Image imageComponent;
    public Text textComponent;
}
public class UnityTextComponents : UnityComponentGroup
{
    public Text textComponent;
}
public class UnityListComponents : UnityComponentGroup
{
    public ScrollRect scrollComponent;
    public List<GameObject> contentListItems;
}


