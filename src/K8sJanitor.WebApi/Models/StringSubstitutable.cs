namespace K8sJanitor.WebApi.Models
{
    public class StringSubstitutable
    {
        private readonly string _name;

        protected StringSubstitutable(string name)
        {
            _name = name;
        }
        
        public static implicit operator string(StringSubstitutable roleName)
        {
            return roleName?._name;
        }
        
        public override string ToString()
        {
            return _name;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType() && obj.GetType() != typeof(string))
            { return false;}
            
            switch (obj)
            {
                case StringSubstitutable stringSubstitutable:
                    return _name.Equals(stringSubstitutable._name);
                case string item:
                    return _name.Equals(item);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}