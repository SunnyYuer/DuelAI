DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
Duel = DuelLayout:GetComponent("DuelEvent")
duelData = Duel.duelData
cost = true

testcard = "71703785"

function c71703785()
    local effect1 = Duel:CreateEffect(1, EffectType.cantrigger)
    effect1:SetCondition()
    effect1:SetCost()
    Duel.thiscard.cardeffect:Add(effect1)
    --[[
    if(c71703785condition1()) then
        Duel:SetCanTriggerEffect(1, cost)
    end
    if(c71703785condition2()) then
        local buff = c71703785buff2()
        Duel:SetContinuousEffect(buff)
    end
    if(c71703785condition3()) then
        Duel:SetCanTriggerEffect(3)
    end
    ]]
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
    if(duelData.duelPhase == GamePhase.damageStepStart) then
    if(Duel:ThisCardIsBattle()) then
    if(Duel:GetAntiMonster().attribute == "暗") then
        return true
    end
    end
    end
    return false
end

function c71703785effect2()
    Duel.thiscard.atk = Duel.thiscard.atk * 2
end

function c71703785buff2()
    local buff = Duel:CreateDuelBuff(2)
    buff:SetConTime(duelData.turnNum, GamePhase.damageStepEnd)
    buff:SetBuff(BuffType.atknew)
    return buff
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
    --Duel:SetStartupEffect(1)
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
    --[[
    if(c77538567condition1()) then
        print("1111111111111判断正确")
        Duel:SetCanTriggerEffect(1)
    end
    ]]
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
    print("1111111111111发动效果")
end
