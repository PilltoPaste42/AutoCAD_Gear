namespace CGPlugin.Models;

using System.ComponentModel.DataAnnotations;

using CGPlugin.Models.CustomDataAnnotations;

/// <summary>
///     Модель для косозубых шестерней
/// </summary>
public class HelicalGearModel
{
    [Required]
    [Range(5, 5000)]
    public uint Diameter { get; set; }

    [Required]
    [HelicalGearModule]
    public uint Module { get; set; }

    [Required]
    [AbsoluteRange(25, 45)]
    public int TeethAngle { get; set; }

    [Required]
    [HelicalGearTeethCount]
    public uint TeethCount { get; set; }

    [Required]
    [Range(5, 5000)]
    public uint Width { get; set; }
}