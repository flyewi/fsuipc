namespace fsuipcserve
{

    //  Example: 
    //  { "panel": "lights","offsetname": "lights","offset": 3340,"type": "BitArray","convert":"lightList"  }   
    public class OffsetMessage
    {
        public string panel { get; set; }           // Gruppierung des Offsets
        public string offsetname { get; set; }      // Name unter dem der Offset im Json zurückgegeben wird
        public int offset { get; set; }             // Offsetnummer
        public string type { get; set; }            // Variablentyp
        public string convert { get; set; }         // Optionale Methode zum konvertieren des Offsetwertes
    }
    
}
