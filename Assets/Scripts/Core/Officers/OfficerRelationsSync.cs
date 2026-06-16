using System;
using System.Collections.Generic;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>人際關係雙向維護：A 親愛 B ⇒ B 親愛 A，義兄弟／配偶／厭惡同理。</summary>
    public static class OfficerRelationsSync
    {
        enum RelationKind
        {
            Beloved,
            Hated,
            SwornBrother,
            Spouse
        }

        /// <summary>替換本將人際並同步 Pool 內對象（需 <paramref name="resolveOfficer"/> 可解析對方）。</summary>
        public static void Apply(Officer officer, OfficerRelations relations, Func<int, Officer> resolveOfficer)
        {
            if (officer == null)
                return;

            RemoveAllSymmetric(officer, resolveOfficer);
            officer.ReplaceLocalRelations(relations);
            EnsureSymmetric(officer, resolveOfficer);
        }

        /// <summary>依本將現有列表補齊對向 id（載入全表後呼叫）。</summary>
        public static void EnsureSymmetric(Officer officer, Func<int, Officer> resolveOfficer)
        {
            if (officer == null || resolveOfficer == null)
                return;

            AddSymmetricForList(officer, officer.BelovedOfficerIds, RelationKind.Beloved, resolveOfficer);
            AddSymmetricForList(officer, officer.HatedOfficerIds, RelationKind.Hated, resolveOfficer);
            AddSymmetricForList(officer, officer.SwornBrotherIds, RelationKind.SwornBrother, resolveOfficer);
            AddSymmetricForList(officer, officer.SpouseOfficerIds, RelationKind.Spouse, resolveOfficer);
        }

        static void RemoveAllSymmetric(Officer officer, Func<int, Officer> resolveOfficer)
        {
            if (resolveOfficer == null)
                return;

            RemoveSymmetricForList(officer, officer.BelovedOfficerIds, RelationKind.Beloved, resolveOfficer);
            RemoveSymmetricForList(officer, officer.HatedOfficerIds, RelationKind.Hated, resolveOfficer);
            RemoveSymmetricForList(officer, officer.SwornBrotherIds, RelationKind.SwornBrother, resolveOfficer);
            RemoveSymmetricForList(officer, officer.SpouseOfficerIds, RelationKind.Spouse, resolveOfficer);
        }

        static void RemoveSymmetricForList(
            Officer self,
            IReadOnlyList<int> ids,
            RelationKind kind,
            Func<int, Officer> resolveOfficer)
        {
            foreach (int otherId in ids)
            {
                if (otherId <= 0 || otherId == self.RuntimeId)
                    continue;
                Officer other = resolveOfficer(otherId);
                if (other == null)
                    continue;
                RemoveLink(other, self.RuntimeId, kind);
            }
        }

        static void AddSymmetricForList(
            Officer self,
            IReadOnlyList<int> ids,
            RelationKind kind,
            Func<int, Officer> resolveOfficer)
        {
            foreach (int otherId in ids)
            {
                if (otherId <= 0 || otherId == self.RuntimeId)
                    continue;
                Officer other = resolveOfficer(otherId);
                if (other == null)
                    continue;
                AddLink(self, other, kind);
            }
        }

        static void AddLink(Officer a, Officer b, RelationKind kind)
        {
            switch (kind)
            {
                case RelationKind.Beloved:
                    a.TryAddBeloved(b.RuntimeId);
                    b.TryAddBeloved(a.RuntimeId);
                    break;
                case RelationKind.Hated:
                    a.TryAddHated(b.RuntimeId);
                    b.TryAddHated(a.RuntimeId);
                    break;
                case RelationKind.SwornBrother:
                    a.TryAddSwornBrother(b.RuntimeId);
                    b.TryAddSwornBrother(a.RuntimeId);
                    break;
                case RelationKind.Spouse:
                    a.TryAddSpouse(b.RuntimeId);
                    b.TryAddSpouse(a.RuntimeId);
                    break;
            }
        }

        static void RemoveLink(Officer target, int otherId, RelationKind kind)
        {
            switch (kind)
            {
                case RelationKind.Beloved:
                    target.RemoveBeloved(otherId);
                    break;
                case RelationKind.Hated:
                    target.RemoveHated(otherId);
                    break;
                case RelationKind.SwornBrother:
                    target.RemoveSwornBrother(otherId);
                    break;
                case RelationKind.Spouse:
                    target.RemoveSpouse(otherId);
                    break;
            }
        }
    }
}
