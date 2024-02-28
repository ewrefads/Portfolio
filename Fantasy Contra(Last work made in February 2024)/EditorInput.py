from GameObject import GameobjectType
import pygame

class EditorInput:
    def __init__(self , leveleditor) -> None:
        self._word =""
        self._argument_index = 1
        self._leveleditor = leveleditor
        self._values = {}
        
        self._font = pygame.font.Font(None, 36)
        
    def string_to_int(self, string):
        value = int(string)
        return value
    
    def string_to_list(self, string):
        string_list = string.split()
        list = [int(x) for x in string_list]
        
        return list
    
    def string_to_bool(self, string):
        b = bool(string)
        return b
    
    #chat gpt kode til at tegne text
    def draw_input(self):
        # Render text onto a surface
        text_surface = self._font.render(self._word, True, (0,0,0))

        # Get the rectangle of the text surface
        text_rect = text_surface.get_rect()

        # Center the text on the screen
        text_rect.center = (300,300)

        # Blit the text onto the screen
        self._leveleditor._gameworld.screen.blit(text_surface, text_rect)
    
    def take_input(self, type):
        self.draw_input()
                
                
        for event in pygame.event.get():
            if event.type == pygame.KEYDOWN:
                if event.key == pygame.K_BACKSPACE:
                        # Remove last character if backspace is pressed
                    if len(self._word) > 0:
                        self._word = self._word[:-1]
                elif event.key == pygame.K_SPACE:
                        # Add space if space is pressed
                    self._word += " "
                elif event.key == pygame.K_RETURN:
                        # Print the formed word if enter is pressed
                    print(self._word)
                    self.correct_input(type,self._word)
                    self._word = ""
                else:
                    # Add the pressed key to the word
                    self._word += event.unicode
                
                
                
                

                
    
    def correct_input(self, type, word):
        
        try:
            match type:
                case GameobjectType.MOVINGPLATFORM:
                    mp = self._leveleditor._last_gameobject.get_component("MovingPlatform")
                    
        
       
        
                    match self._argument_index:
                        case 1:
                            #expected length
                            list_length = 2
                            
                            #funktion
                            list = self.string_to_list(word)
                            
                            #checker
                            if(len(list)<list_length):
                                raise ValueError("")
                            
                            #tilføjer værdier
                            self._values['endpos'] = list
                            mp.endpoint = pygame.Vector2( mp.startpoint.x + list[0],mp.startpoint.y+ list[1])
                            
                            print("enter endpointdelay:  fx 2")
                        case 2: 
                            num = self.string_to_int(word)
                            self._values['endpointdelay'] = num
                            mp.endpoint_delay = num
                            
                            print("enter speed:  fx 5")
                        case 3: 
                            num = self.string_to_int(word)
                            self._values['speed'] = num
                            
                            mp.speed = num
                            print("enter active:  fx False")
                        case 4: 
                            b = self.string_to_bool(word)
                           
                            self._values['active'] = b
                            mp.active = b
                            
                        case 5:
                            self.cleanup()
        
            self._argument_index+=1
            
        except Exception as e:
            print("forkert input")
                            
                        
    def cleanup(self):
        self._leveleditor._last_dictionary.update(self._values)
        #print(self._leveleditor._last_dictionary)
        self._leveleditor.save_object(self._leveleditor._last_dictionary)
        #print(self._leveleditor._levels[self._leveleditor._scene][len(self._leveleditor._levels[self._leveleditor._scene])-1])
        self._values = {}
        self._word = ""
        self._argument_index = 1
        
        self._leveleditor._giving_consoleinput = False
                     
                   
 