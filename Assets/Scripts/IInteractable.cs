using System.Collections.Generic;

public interface IInteractable {
    // Vrátí seznam dostupných akcí pro tento objekt
    List<InteractionAction> GetInteractions();
}