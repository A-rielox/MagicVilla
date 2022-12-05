using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace MagicVilla_WebAPI.Models;

public class VillaNumber
{
    // p' q sea primary-key
    // DatabaseGenerated.. para q no se genere sola en la db
    [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int VillaNo { get; set; }
    public string SpecialDetails { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // VillaID es una foreign key, q va a apuntar a una tabla q se llama Villa
    // se ponen las 2 lineas juntas, la de abajo es la mencion a la tabla, asi se 
    // deja expresado que VillaID es una foreign-key a la tabla Villa
    // el normal con MAGIC STRING "[ForeignKey("VillaID"))]" no avisa si hay
    // error xq cambie el nombre o algo asi, xeso se ocupa mejor este
    [ForeignKey(nameof(VillaID))]
    public int VillaID { get; set; }
    public Villa Villa { get; set; } // navigation prop
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