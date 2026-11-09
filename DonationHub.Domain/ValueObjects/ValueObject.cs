namespace DonationHub.Domain.ValueObjects
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();
        public override bool Equals(object? obj) 
        {
            if (obj == null || obj is not ValueObject)
                return false;
            ValueObject other = (ValueObject)obj;
            if (GetType() != other.GetType())
                return false;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }
        public override int GetHashCode() 
        {
            var hash = new HashCode();
            foreach (var component in GetEqualityComponents()) 
            {
                hash.Add(component);
            }
            return hash.ToHashCode();
        }
    }
}
