from Components import Component, SpriteRenderer
from Builder import Builder
import pygame
from GameObject import GameObject

class HUD(Component):
    def __init__(self, gameworld) -> None:
        super().__init__()
        self._gameworld = gameworld
        #player score
        self._score = 0
        #player health
        self._health = 0
        #mana bar
        self._mana = 0
        
        #ui in HUD
        self._ui_elements = []
        
    def add_ui(self, ui, name):
        self._ui_elements.append(ui)
        #self._gameworld.instantiate_UI(ui)
        self.gameObject.add_child(ui, name)
    
    def score_change(self, value):
        score_text = self.gameObject.get_child("score text")
        text = score_text.get_component("Text")
        text.text = str(value)
    
    def health_change(self, value):
        image = self.gameObject.get_child("health image")
        text = image.get_component_in_children("Text")
        text[0].text = str(value)
    
    def mana_change(self, value):
        mana_slider = self.gameObject.get_child("mana slider")
        slider = mana_slider.get_component("Slider")
        slider.value = value
    
    def max_mana_change(self, value):
        mana_slider = self.gameObject.get_child("mana slider")
        slider = mana_slider.get_component("Slider")
        slider.max_value = value
        
    def awake(self, game_world):
        pass
    
    def start(self):
        pass
    
    def update(self,  delta_time):
        for element in self._ui_elements:
            element.update(delta_time)
    
class Text(Component):
    def __init__(self, text, text_size, prefix) -> None:
        super().__init__()
        self._text_prefix = prefix
        self._text = text
        self._text_size = text_size
        self._font = pygame.font.Font(None, text_size)
        
    @property
    def text(self):
        return self._text    
    
    @text.setter
    def text(self, new_value):
        self._text = new_value
    
    def awake(self, game_world):
        self._game_world = game_world
    
    def start(self):
        pass
    
    def update(self,  delta_time):

        text_surface = self._font.render(self._text_prefix + self._text, True, (0,0,0))
        text_rect = text_surface.get_rect()
        text_rect.center = self.gameObject.transform.position
        self._game_world.screen.blit(text_surface, text_rect)
    
class Slider(Component):
    def __init__(self, start_value, max_value, gameworld) -> None:
        super().__init__()
        self._gameworld = gameworld
        
        self._value = start_value
        self._max_value = max_value
        
        self._slider_text = None
        self._slider_background = None
        self._slider_fill = None
        
        self._slider_max_size = 1
        self._fill_sr = None
        
    @property
    def max_value(self):
        return self._max_value
    
    @max_value.setter
    def max_value(self, new_value):
        self._max_value = new_value 
    
    @property
    def value(self):
        return self._value
    
    @value.setter
    def value(self, new_value):
        self._value = new_value
        self.update_slider()
        
    def slider_setup(self):
        
        sb = SliderBuilder(self.gameObject.transform.position)
        sb.build()
        self._slider_text = sb.get_text()
        self._slider_background = sb.get_background()
        self._slider_fill = sb.get_fill()
        
        self.gameObject.add_child(self._slider_background, "background")
        
        #print(self._slider_background.components)
        bg_sr = self._slider_background.get_component("SpriteRenderer")
        self._slider_max_size = bg_sr._sizeX_scale
        self._fill_sr = self._slider_fill.get_component("SpriteRenderer")
        
        self._text = self._slider_text.get_component("Text")
        
        
        self.update_slider()
    
    #chatgpt funktion til at scalere et tal fra et forhold til et andet
    def map_value(self,value, from_min, from_max, to_min, to_max):
        return (value - from_min) * (to_max - to_min) / (from_max - from_min) + to_min  
    
    def update_slider(self):
        current_scale = self._fill_sr._sizeY_scale
        new_sizeX = self.map_value(self._value, 0, self._max_value, 0, self._slider_max_size)
        self._fill_sr.set_scale(new_sizeX,current_scale)
        
        self._text.text = str(self._value) +"/"+ str(self._max_value)
        
    
    def awake(self, game_world):
        pass
    
    def start(self):
        pass
    
    def update(self,  delta_time):
        pass


class Button(Component):
    def __init__(self, gameworld,function_to_execute) -> None:
        super().__init__()
        self._clicking_rect = None
        self._mouse_pos = pygame.Vector2(0,0)
        self._active = True
        self._clicked = False
        
        self._gameworld = gameworld
        self._function = function_to_execute
    
    def awake(self, game_world):
        pass
    
    def start(self):
        #self.button_rect()
        pass
        
    def button_rect(self):
        sr = self.gameObject.get_component("SpriteRenderer")
        size = sr.get_scale()
        #makes rect that is clicking zone
        self._clicking_rect = pygame.Rect(
            self.gameObject.transform.position[0]+self._gameworld.camera_offset[0],
            self.gameObject.transform.position[1]+self._gameworld.camera_offset[1],
            size[0],
            size[1]
            )
    
    def update(self,  delta_time):
        mouse_buttons = pygame.mouse.get_pressed()

        #Check if the left mouse button is clicked
        if mouse_buttons[0]:
            self.check_if_clicked()
            
        #checker om man stopper med at trykke på knappen og gør den klar til at blive trykket på igen
        elif (self._clicked is True):
            self._clicked = False
                
        
        
    def check_if_clicked(self):
        #får muse position og laver et lille rect rundt om som checker for collision med knappen
        self._mouse_pos = pygame.mouse.get_pos() + self._gameworld.camera_offset
        mouse_rect = pygame.Rect(self._mouse_pos[0],self._mouse_pos[1], 5,5)
        self.button_rect()
        
        #checker om der er en collision
        is_rect_colliding = self._clicking_rect.colliderect(mouse_rect)
        #hvis der er en collision så er der klikke på knappen
        if(is_rect_colliding  and not self._clicked):
            self.clicked()  
    
    def clicked(self):
        self._clicked = True
        #kalder den funktion som er blevet gevet som argrument i konstruktoren
        self._function()
        pass
    

    
    
    
    
    
    
    
    


#a lot of messy code to setup a slider
class SliderBuilder(Builder):
    def __init__(self, spawnposition) -> None:
        self._spawnposition = spawnposition
        
        self._text = None
        self._background = None
        self._fill = None

    def build(self):
        scaleY = 0.2
        
        #builds background
        self._background = GameObject(self._spawnposition)
        bs = self._background.add_component(SpriteRenderer("whiteSquare.png"))
        bs.follows_camera=True
        bs.change_color((255,255,255,255))
        bs._sizeY_scale = scaleY
        
        #builds fill
        self._fill = GameObject(pygame.Vector2(0,0))
        fs = self._fill.add_component(SpriteRenderer("whiteSquare.png"))
        fs.follows_camera=True
        fs.change_color((0,0,255,255))
        fs._sizeY_scale = scaleY
        
        #builds text
        text_position = bs.get_scale()
        
        
        self._text = GameObject(pygame.Vector2(text_position[0]/2,text_position[1]/2))
        self._text.add_component(Text("50/100", 30, "mana"))
        
        self._background.add_child(self._fill, "slider fill")
        self._background.add_child(self._text, "slider text")
        
        

    def get_text(self) -> GameObject:
        return self._text
    
    def get_background(self) -> GameObject:
        return self._background
    
    def get_fill(self) -> GameObject:
        return self._fill