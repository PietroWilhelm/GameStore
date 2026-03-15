using GameStore.Domain.Commom;
using GameStore.Domain.Helpers;

namespace GameStore.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    
    public String Cpf { get; private set; } // Usando String por int no C# suporta até 2.147.483.647, o que não é suficiente para CPFs que começam com números altos
    private DateOnly SetBirhDate { get;  set; }
    public string Password { get; private set; }
    private string Salt { get; set; }
    
    //1..1
    public CustomerConfiguration Configuration { get; set; }

    public Customer(string name, string email, DateOnly dateBorn, string rawPassword, int cpf)
    {
        UpdateName(name);
        UpdateEmail(email);
        SetBirthDate(dateBorn);
        ChangePassword(rawPassword);
        
    }
    
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new Exception("Nome não pode ser vazio.");
        
        Name = newName;
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@"))
            throw new Exception("E-mail inválido.");
            
        Email = newEmail;
    }

    public void SetBirthDate(DateOnly newDate)
    {
        var age = CalculateAge(newDate);
        
        if (age < 13)
            throw new Exception("Usuário deve ter pelo menos 13 anos.");

        SetBirhDate = newDate;
    }

    public void ChangePassword(string newRawPassword)
    {
        if (string.IsNullOrWhiteSpace(newRawPassword) || newRawPassword.Length < 8)
            throw new Exception("A senha deve ter pelo menos 8 caracteres.");

        Salt = Guid.NewGuid().ToString("N"); 
        
        Password = HashHelper.Hash(newRawPassword, Salt);
    }
    
    public int Age => CalculateAge(SetBirhDate);

    private static int CalculateAge(DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - date.Year;
        if (date > today.AddYears(-age)) age--;
        return age;
    }
     
}