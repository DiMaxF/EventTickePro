using System;

[Serializable]
public class EmailModel 
{
    public string name;
    public string email;

    public EmailModel(string name, string email) 
    {
        this.name = name;
        this.email = email;
    }
}
