using System.ComponentModel.DataAnnotations;

namespace MagicVilla_WebAPI.Models
{
    public class Villa
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Details { get; set; }
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string ImageUrl { get; set; }
        public string Amenity { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // para especificar la relacion one-to-many ( una villa muchas villaNumber )
        public virtual IList<VillaNumber> VillaNumbers { get; set; }
    }
}

// ERROR EN LA MIGRACION al agregar la foreign-key
//
// The ALTER TABLE statement conflicted with the FOREIGN KEY constraint
// "FK_VillaNumbers_Villas_VillaID". The conflict occurred in database "Magic_VillaAPI",
// table "dbo.Villas", column 'Id'.
// 
// xq las villanumbers q ya estaban no tenian "VillaID" , => lo tomaba como null y no sabia a cual
// villa coresponde null 
// tuve q borrar las villasnumber y ya