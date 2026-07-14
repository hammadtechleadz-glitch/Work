namespace project02.Models
{
    public class NoteTag
    {
        public int NoteId { get; set; }

        public int TagId { get; set; }

        public virtual Notes note { get; set; }

        public virtual Tag tag { get; set; }// wehn we link a note a to tag ot tags, only then this model gets polulated and tha is done inthe notes controller


    }
}
