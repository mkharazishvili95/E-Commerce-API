namespace E_Commerce_API.Models
{
    public class BankModel
    {
        public int Id { get; set; }
        public double BankStatement { get; set; } 
        public BankModel() {
            BankStatement = 0;
        }
    }
}
