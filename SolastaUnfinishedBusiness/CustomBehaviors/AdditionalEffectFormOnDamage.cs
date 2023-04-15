using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomValidators;

public delegate IEnumerable<EffectForm> AdditionalEffectFormOnDamageHandler(
    GameLocationCharacter attacker, GameLocationCharacter defender, IAdditionalDamageProvider provider);
