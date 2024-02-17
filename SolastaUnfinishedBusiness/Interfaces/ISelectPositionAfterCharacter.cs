﻿using SolastaUnfinishedBusiness.Builders;

namespace SolastaUnfinishedBusiness.Interfaces;

internal static class SelectPositionAfterCharacter
{
    internal const string ConditionSelectedCharacterName = "ConditionSelectedCharacter";

    internal static readonly ConditionDefinition ConditionSelectedCharacter = ConditionDefinitionBuilder
        .Create(ConditionSelectedCharacterName)
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AddToDB();
}

public interface ISelectPositionAfterCharacter
{
    public int PositionRange { get; }
}
