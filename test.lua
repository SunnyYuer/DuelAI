DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
DuelOperation = DuelLayout:GetComponent("DuelOperation")

function Card71703785()
    if(DuelOperation:DrawThisCard("71703785")) then
        DuelOperation:SetChainableEffect(1)
    else
        print("没有抽到马哈德")
    end
end

function Card71703785Effect1()
    print("马哈德发动效果")
end