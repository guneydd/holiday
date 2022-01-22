using System.ComponentModel.DataAnnotations;

namespace Holidays.Api.Models {
    public class Country {
        [Key]
        public string Code { get; set; }

        public string Name { get; set; }

        public Country() { }

        public Country(string _code, string _name) {
            Name = _name;
            Code = _code;
        }
    }
}