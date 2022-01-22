using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Holidays.Api.Models;

public class DayStatus {
    public enum EStatus {
        WorkDay,
        FreeDay,
        Holiday,
        Invalid
    }


    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    public string Country { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public EStatus Status { get; set; }
}