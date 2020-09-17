DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
Duel = DuelLayout:GetComponent("DuelEvent")
duelData = Duel.duelData
cost = true

testcard = "71703785,72892473,77538567"

function c71703785()
    local effect1 = Duel:CreateEffect(EffectType.cantrigger)
    effect1:SetCondition()
    effect1:SetCost()

    local effect2 = Duel:CreateEffect(EffectType.continuous)
    effect2:SetCondition()
    effect2:SetConTime(0, GamePhase.damageStepEnd)

    local effect3 = Duel:CreateEffect(EffectType.cantrigger)
    effect3:SetCondition()
end

function c71703785condition1()
    return Duel:InTimePoint(Duel.thiscard, GameEvent.drawcard)
end

function c71703785cost1()
    Duel:ShowCard(Duel.thiscard)
end

function c71703785effect1()
    Duel:SpecialSummon(Duel.thiscard)
end

function c71703785condition2()
    if(duelData.duelPhase >= GamePhase.damageStepStart and duelData.duelPhase <= GamePhase.damageStepEnd) then
    if(Duel:ThisCardIsBattle()) then
    if(Duel:GetAntiMonster().attribute == "暗") then
        return true
    end
    end
    end
    return false
end

function c71703785effect2()
    Duel:AttackNew(Duel.thiscard.atk * 2)
end

function c71703785condition3()
    if(Duel:InCase(Duel.thiscard, GameEvent.battledestroy)) then
        return true
    end
    if(Duel:InCase(Duel.thiscard, GameEvent.effectdestroy)) then
        return true
    end
    return false
end

function c71703785effect3()
    local targetcard = TargetCard.New()
    targetcard:SetPosition(CardPosition.handcard)
    targetcard:SetPosition(CardPosition.deck)
    targetcard:SetPosition(CardPosition.grave)
    targetcard:SetTarget(GameCard.name, "黑魔术师")
    Duel:SelectCard(targetcard, 1, GameEvent.specialsummon)
end

function c72892473()
    local effect1 = Duel:CreateEffect(EffectType.startup)
end
 
function c72892473effect1()
    local player = Duel:GetPlayerOrder()
    local num = {}
    for i = 0, player.Count-1 do
        num[i] = Duel:DisCardAll(player[i])
    end
    Duel:AfterThat()
    for i = 0, player.Count-1 do
        Duel:DrawCard(num[i], player[i])
    end
end

function c77538567()
    local effect1 = Duel:CreateEffect(EffectType.cantrigger)
    effect1:SetCondition()
end

function c77538567condition1()
    local targetcard = TargetCard.New()
    targetcard:SetSide(PlayerSide.ops)
    targetcard:SetTarget(GameCard.type, "魔法")
    targetcard:SetTarget(GameCard.type, "陷阱")
    if(Duel:InTimePoint(targetcard, GameEvent.activatecard)) then
        return true
    end
    return false
end

function c77538567effect1()
    Duel:ActivateInvalid()
end
