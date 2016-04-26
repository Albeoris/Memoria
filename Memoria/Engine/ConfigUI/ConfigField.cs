using UnityEngine;

#pragma warning disable 169
#pragma warning disable 414
#pragma warning disable 649

// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable ConvertToConstant.Global
// ReSharper disable UnusedParameter.Global
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

public class ConfigField
{
    public ConfigUI.Configurator Configurator;
    public GameObject ConfigParent;
    public ButtonGroupState Button;
    public BetterList<GameObject> ConfigChoice;
    public bool IsSlider;
    public float Value;

    public ConfigField()
    {
        ConfigChoice = new BetterList<GameObject>();
        Value = -1f;
    }
}