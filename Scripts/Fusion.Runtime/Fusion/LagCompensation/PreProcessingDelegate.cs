using System.Collections.Generic;

namespace Fusion.LagCompensation;

public delegate void PreProcessingDelegate(ref Query query, List<HitboxRoot> rootCandidates, HashSet<int> processedColliderIndices);
