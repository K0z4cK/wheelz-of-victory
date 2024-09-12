using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlacingPanel : MonoBehaviour
{
    private UnityAction onPlacingEnd;

    [SerializeField]
    private GameObject _panel;
    [SerializeField]
    private GameObject _notEnoughMoneyPanel;

    [SerializeField]
    private Button _mageButton;
    [SerializeField]
    private Button _dwarfButton;
    [SerializeField]
    private Button _cancelButton;

    [SerializeField]
    private TMP_Text _magePrice;
    [SerializeField]
    private TMP_Text _dwarfPrice;

    [SerializeField]
    private Unit _magePrefab;
    [SerializeField]
    private Unit _dwarfPrefab;

    [SerializeField]
    private Transform _alliesPlaceholder;

    private Vector2 _placePoint;

    private void Awake()
    {
        _magePrice.text = _magePrefab.Price.ToString();
        _dwarfPrice.text = _dwarfPrefab.Price.ToString();
        _dwarfButton.onClick.AddListener(delegate { PlaceUnit(_dwarfPrefab); });
        _mageButton.onClick.AddListener(delegate { PlaceUnit(_magePrefab); });
        _cancelButton.onClick.AddListener(CancelPlacing);
    }

    public void ShowPanel(Vector2 placePoint, UnityAction endPlacing)
    {
        onPlacingEnd = endPlacing;

        Time.timeScale = 0;
        _placePoint = placePoint;
        _panel.SetActive(true);
    }

    private void PlaceUnit(Unit unit)
    {
        if (!LevelController.Instance.DeductMoney(unit.Price))
        {
            _notEnoughMoneyPanel.SetActive(true);
            return;
        }

        Instantiate(unit, _placePoint, unit.transform.rotation, _alliesPlaceholder);
        _panel.SetActive(false);
        Time.timeScale = 1;
        onPlacingEnd?.Invoke();
    }

    private void CancelPlacing()
    {
        _panel.SetActive(false);
        _notEnoughMoneyPanel.SetActive(false);
        Time.timeScale = 1;
        onPlacingEnd?.Invoke();
    }

}
