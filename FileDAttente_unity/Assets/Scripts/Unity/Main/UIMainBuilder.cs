using UnityEngine;
using System.IO;
using System.Reflection;

public class UIMainBuilder : MonoBehaviour
{
    //public DataBaseSave savedDatabase;
    public string savePath = @"C:\Users\anteo\DevLand\TestDatabase.json";

    private UIDeskBuilder deskBuilder;
    private UITabSelectorBuilder tabSelectorBuilder;

    public UIMain CurrentUI { get; private set; }

    private void Awake()
    {
        deskBuilder = GetComponentInChildren<UIDeskBuilder>(true);
        tabSelectorBuilder = GetComponentInChildren<UITabSelectorBuilder>(true);

        //Database currentDatabase = savedDatabase != null ? savedDatabase.data : new Database();
        //CurrentUI = new UIMain(currentDatabase);
        //DatapackSerializer.Serialize(currentDatabase, databaseSavePath);

        ///Database currentDatabase = DatapackSerializer.Deserialize(databaseSavePath);

        savePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Save";
        CurrentUI = new UIMain(savePath);
    }

    private void OnEnable()
    {
        if (CurrentUI != null)
        {
            if (tabSelectorBuilder != null)
                tabSelectorBuilder.Build(CurrentUI.TabSelector);

            if (deskBuilder != null)
                deskBuilder.Build(CurrentUI.Desks[CurrentUI.TabSelector.CurrentSelection]);

            CurrentUI.onSwitchDesk += OnSwitchDesk;
            //CurrentUI.onSaveDatabase += OnEndEditDatabase;
        }
    }
    private void OnDisable()
    {
        if (CurrentUI != null)
        {
            if (deskBuilder != null)
                deskBuilder.Clear();
            
            CurrentUI.onSwitchDesk -= OnSwitchDesk;
            //CurrentUI.onSaveDatabase -= OnEndEditDatabase;
        }
    }

    private void OnSwitchDesk(UICardDesk newDesk)
    {
        if (deskBuilder != null)
            deskBuilder.Build(newDesk);
    }

    //private void OnEndEditDatabase(Database editedDatabase)
    //{
    //    if (savedDatabase != null)
    //        savedDatabase.data = editedDatabase;

    //    DatapackSerializer.Serialize(editedDatabase, databaseSavePath);
    //}
}
