

public class Session{
    public int port {get; set;}
    public string name {get; set;}
    public int pid {get; set;}
    public Session(string _name, int _port, int _pid)
    {
        name =_name; port = _port; pid = _pid;
    }
}