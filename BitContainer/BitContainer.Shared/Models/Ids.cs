

using System;

namespace BitContainer.Shared.Models
{


    public struct CStorageEntityId : IEquatable<CStorageEntityId>
    {
        
        public static readonly CStorageEntityId RootId = new CStorageEntityId(Guid.Empty);

        public Boolean IsRootId => this.Equals(RootId);

        public void IfRootId(Action action)
        {
            if (IsRootId) action();
        }

        public void IfNotRootId(Action action)
        {
            if (!IsRootId) action();
        }

        
        private readonly Guid _id;

        public CStorageEntityId(Guid id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return _id.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CStorageEntityId)) return false;
            return Equals((CStorageEntityId) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public bool Equals(CStorageEntityId obj)
        {
            return _id.Equals(obj._id);
        }

        public static bool operator==(CStorageEntityId obj1, CStorageEntityId obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator!=(CStorageEntityId obj1, CStorageEntityId obj2)
        {
            return !(obj1 == obj2);
        }

        public Guid ToGuid()
        {
            return _id;
        }
    }


    public struct CUserId : IEquatable<CUserId>
    {
        
        private readonly Guid _id;

        public CUserId(Guid id)
        {
            _id = id;
        }

        public override string ToString()
        {
            return _id.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CUserId)) return false;
            return Equals((CUserId) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public bool Equals(CUserId obj)
        {
            return _id.Equals(obj._id);
        }

        public static bool operator==(CUserId obj1, CUserId obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator!=(CUserId obj1, CUserId obj2)
        {
            return !(obj1 == obj2);
        }

        public Guid ToGuid()
        {
            return _id;
        }
    }

    
}
