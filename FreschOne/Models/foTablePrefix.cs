namespace FreschOne.Models
{
    public class foTablePrefix
    {
        public long ID { get; set; }        // ID is a bigint, the primary key
        public string Prefix { get; set; }  // Prefix is a varchar(100), e.g., "tbl_tran_"
        public string Description { get; set; }  // Description is a varchar(255), e.g., "Transactional"
        public bool Active { get; set; }    // Active is a BIT (boolean), 1 or 0
    }

}
