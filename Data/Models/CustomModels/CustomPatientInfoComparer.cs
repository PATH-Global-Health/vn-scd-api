using Data.Entities.SMDEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Data.Models.CustomModels
{
    public class PatientInfoComparer : IEqualityComparer<PatientInfo>
    {
        public bool Equals([AllowNull] PatientInfo x, [AllowNull] PatientInfo y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.CBOId == y.CBOId && x.ReachCode == y.ReachCode) return true;
            return false;
        }

        public int GetHashCode([DisallowNull] PatientInfo obj)
        {
            var val = obj.CBOId.ToString() + obj.ReachCode;
            return val.GetHashCode();
        }
    }
}
