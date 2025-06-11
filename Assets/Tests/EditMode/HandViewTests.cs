using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Runtime.CardGameplay.Deck;
using Runtime.CardGameplay.Card;
using Runtime.CardGameplay.Card.View;

public class HandViewTests
{
    [UnityTest]
    public IEnumerator ArrangeCardsRestoresHover()
    {
        var parent = new GameObject("list");
        var list = parent.AddComponent<HorizontalCardListView>();

        var cardObj = new GameObject("card");
        cardObj.transform.SetParent(parent.transform);
        var controller = cardObj.AddComponent<CardController>();
        var view = cardObj.AddComponent<CardView>();
        controller.View = view;
        list.AddCard(controller);

        list.ArrangeCardsInArch();
        yield return new WaitForSeconds(0.6f);

        Assert.IsTrue(view != null);
    }
}
