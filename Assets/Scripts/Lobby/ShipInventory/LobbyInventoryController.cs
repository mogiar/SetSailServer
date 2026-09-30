using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyInventoryController
{
    private LobbyInventoryModel _model;
    private LobbyInventoryView _view;

    public void Init(LobbyInventoryView view)
    {
        _view = view;
        _model = new LobbyInventoryModel();
        _view.Init(_model);
    }

    public void OnFeatureDataRecieved()
    {
        // update model
    }
}

