# Cult-Penguin

Liste af Metoder til spil Kommunikation

MovePlayer(string name, Vector2 destination)
Fortæller hvor en spiller med brugernavnet 'name' skal bevæge sig hen til 'destination'

ChatHandler.Instance.DisplayMessage(string name, string message)
Fortæller at spilleren med brugernavnet 'name' har skrevet beskeden 'message'

GameObject player
Spilleren 'player' der er logget ind i den lokale klient.

List< string >  localMessages 
Liste af beskeder den lokale bruger har skrevet siden der sidst blev uploadet beskeder

Dictionary< string, GameObject > onlinePlayers
En dictionary bestående af alle spillere dere er online. Keyen er deres username og Gameobjectet er det tilhørende Gameobject
