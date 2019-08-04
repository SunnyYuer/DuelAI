DuelLayout = UnityEngine.GameObject.Find("DuelLayout(Clone)")
DuelOperation = DuelLayout:GetComponent("DuelOperation")

function Card71703785()
    if(DuelOperation:DrawThisCard("71703785")) then
        print("抽到了马哈德")
    else
        print("没有抽到马哈德")
    end
end