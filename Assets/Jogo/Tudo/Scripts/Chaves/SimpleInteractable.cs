using UnityEngine;

public class SimpleMessageInteractable : Interactable
{
    [Tooltip("Mensagem que será exibida no console ao interagir")]
    public string consoleMessage = "Interação básica realizada";

    [Tooltip("Mensagem que será exibida na UI ao mirar no objeto")]
    public string interactionMessage = "Interagir";

    public override string GetInteractionMessage()
    {
        // Usa a mensagem customizada se existir, senão usa a padrão
        return string.IsNullOrEmpty(customMessage) ? interactionMessage : customMessage;
    }

    public override void Interact()
    {
        // Executa a lógica base da classe Interactable
        base.Interact();
        
        // Adiciona a mensagem específica no console
        Debug.Log(consoleMessage);
        
        // Aqui você pode adicionar outros efeitos como:
        // - Tocar um som
        // - Disparar uma animação
        // - Ativar/Desativar outros objetos
    }
}