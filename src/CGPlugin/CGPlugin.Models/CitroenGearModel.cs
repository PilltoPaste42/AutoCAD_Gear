namespace CGPlugin.Models;

/// <summary>
///     Модель шевронной шестерни
/// </summary>
public class CitroenGearModel
{
    public uint Diameter { get; set; }
    public uint Module { get; set; }
    public int TeethAngle { get; set; }
    public uint TeethCount { get; set; }
    public uint Width { get; set; }
}