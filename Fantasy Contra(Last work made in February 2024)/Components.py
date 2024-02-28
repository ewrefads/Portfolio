from abc import ABC, abstractmethod
import pygame
import os

dir_prefix = ""

class Component(ABC):

    def __init__(self) -> None:
        super().__init__()
        self._gameObject = None
        self._is_active = True

    @property
    def gameObject(self):
        return self._gameObject
    
    @gameObject.setter
    def gameObject(self,value):
        self._gameObject = value
    
    
    def get_copy(self):
        pass
    
    #unik værdi der beskriver specefik ekstra data
    
    def value(self):
        return None

    @abstractmethod
    def awake(self, game_world):
        pass
    
    @abstractmethod
    def start(self):
        pass

    @abstractmethod
    def update(self, delta_time):
        pass

class Transform(Component):

    def __init__(self, position) -> None:
        super().__init__()
        self._position = position

    @property
    def position(self):
        parentpos = self.parent_position()
        return self._position + parentpos
    
    @position.setter
    def position(self,value):
        self._position = value
        
    @property
    def local_position(self):
        return self._position
        
    def get_copy(self):
        t = Transform(self._position)
        return t
    
    #hvis objektet er et child objekt så ligges parent positionen oven i child positionen
    def parent_position(self):
        if(self.gameObject.parent is not None):
            return self.gameObject.parent.transform.position
        else:
            return pygame.Vector2(0,0)

    def translate(self, direction):
        self._position += direction
        
    def awake(self, game_world):
        pass

    def start(self):
        pass
   
    def update(self, delta_time):
        pass

class SpriteRenderer(Component):

    flipped = False
    render = True
    def __init__(self, sprite_name) -> None:
        super().__init__()
        self._sprite_image = pygame.image.load(f"{dir_prefix}Assets\\{sprite_name}").convert_alpha()
    
    #nedarvning fra (pygame.sprite.Sprite) er så kameraet virker
        self._sprite = pygame.sprite.Sprite()
        self._sprite.rect = self._sprite_image.get_rect()
        self._sprite_mask = pygame.mask.from_surface(self.sprite_image)
        self._sprite_name = sprite_name
        self._sprite._layer = 1
        
        
        #customizable options
        self._sizeX_scale = 1
        self._sizeY_scale = 1
        
        self._follows_camera = False
        
        self._color = (255,255,255,255)
        self.change_color(self._color)
        
        self._sizeX = self.sprite.rect.width
        self._sizeY = self.sprite.rect.height

    @property
    def sprite_image(self):
        return self._sprite_image
    
    @property
    def sprite_mask(self):
        return self._sprite_mask
    
    @sprite_image.setter
    def sprite_image(self, value):
        self._sprite_image= value

    @property
    def sprite(self):
        return self._sprite
    
    @property
    def follows_camera(self):
        return self._follows_camera
    
    @follows_camera.setter
    def follows_camera(self, value):
        self._follows_camera = value
        
    def set_sprite_layer(self, value):
        self._sprite._layer = value
        
    
    def set_scale(self, x,y):
        self._sizeX_scale = x
        self._sizeY_scale = y
    
    def get_scale(self):
        return pygame.Vector2(
            int(self._sizeX * self._sizeX_scale),
            int(self._sizeY* self._sizeY_scale)
            )
    def get_scalars(self):
        return pygame.Vector2(self._sizeX_scale, self._sizeY_scale)
    def get_copy(self):
        sr = SpriteRenderer(self._sprite_name)
        return sr
    
    def change_color (self, new_color):
        self._color = new_color
        
        colorImage = pygame.Surface(self._sprite_image.get_size()).convert_alpha()
        colorImage.fill(self._color)
        self._sprite_image.blit(colorImage, (0, 0), special_flags=pygame.BLEND_RGB_MULT)
   
    def awake(self, game_world):
      self._game_world = game_world
      self._sprite.rect.topleft = self.gameObject.transform.position

    def start(self):
        pass
   
    def update(self, delta_time):
        
        self._sprite.rect.topleft = self.gameObject.transform.position
        #if self.flipped:
        #    self._game_world.screen.blit(pygame.transform.flip(self._sprite_image, True, False), self._sprite.rect)
        #else:
        #    self._game_world.screen.blit(self._sprite_image,self._sprite.rect)


        
       
        
        #ændre scalen på det tegnede sprite
        scaled_image = pygame.transform.scale(self._sprite_image,
                                       (
                                        int(self._sizeX * self._sizeX_scale),
                                        int(self._sizeY* self._sizeY_scale)
                                       )
                                        )
        
        offset = pygame.Vector2(0,0)                             
        
        #tegner på verden ud fra kamera offset
        if(not self._follows_camera):
             #får kamera offset så sprites kan tegne ud fra kamera position
            offset = self._game_world.camera_offset
        
        if self.render:
            self._game_world.screen.blit(pygame.transform.flip(scaled_image, self.flipped, False), (self._sprite.rect.x - offset.x, self._sprite.rect.y - offset.y))


    
class Animator(Component):

    def __init__(self) -> None:
        super().__init__()
        self._animations = {}
        self._current_animation = None
        self._animation_time = 0
        self._current_frame_index = 0
        self._subscribers = []

    def add_animation(self, name, *args):
        frames =[]
        for arg in args:
            sprite_image = pygame.image.load(f"{dir_prefix}Assets/{arg}").convert_alpha()
            frames.append(sprite_image)
        
        self._animations[name] = frames
    
    def play_animation(self, animation):
        self._current_animation = animation

    @property
    def current_animation(self):
        return self._current_animation

    def awake(self, game_world):
        self._sprite_renderer = self._gameObject.get_component("SpriteRenderer")

    
    def start(self):
        pass

    def update(self, delta_time):
        self.frame_duration = 0.1

        self._animation_time += delta_time

        #skal vi skifte frame
        if self._animation_time >= self.frame_duration:
            self._animation_time = 0
            self._current_frame_index += 1
            
            #får vi fat på vores aimation
            animation_sequence = self._animations[self._current_animation]

            if self._current_frame_index >= len(animation_sequence):
                self._current_frame_index = 0 #Resetter vores animation
                self.finished_animation()
            
            #Skifter til en ny sprite
            self._sprite_renderer.sprite_image = animation_sequence[self._current_frame_index]
    
    def animation_finish_subscribe(self, sub):
        self._subscribers.append(sub)

    def finished_animation(self):
        for sub in self._subscribers:
            sub(self._current_animation)



class Collider(Component):

    def __init__(self) -> None:
        super().__init__()
        self._other_colliders = []
        self._other_masks = []
        self._listeners = {}

    def awake(self, game_world):
        sr = self.gameObject.get_component("SpriteRenderer")
        if sr.get_scalars().x != 1 or sr.get_scalars().y != 1:
            self._collision_box = sr.sprite.rect.scale_by(sr.get_scalars().x, sr.get_scalars().y)
        else:
            self._collision_box = sr.sprite.rect
        self._sprite_mask = sr.sprite_mask
        game_world.colliders.append(self)

    @property
    def collision_box(self):
        return self._collision_box
    
    @property 
    def sprite_mask(self):
        return self._sprite_mask

    @property
    def Surface(self):
        return self._surface
    

    def subscribe(self, service, method):
        self._listeners[service] = method
    

    def collision_check(self, other):
        is_rect_colliding = self._collision_box.colliderect(other.collision_box)
        is_already_colliding = other in self._other_colliders

        if is_rect_colliding:
            if not is_already_colliding:
                self.collision_enter(other)
                other.collision_enter(self)
            if self.check_pixel_collision(self._collision_box, other.collision_box,self._sprite_mask, other.sprite_mask):
                if other not in self._other_masks:
                    self.pixel_collision_enter(other)
                    other.pixel_collision_enter(self)
                
            else:
                if other in self._other_masks:
                    self.pixel_collision_exit(other)
                    other.pixel_collision_exit(self)
        else:
            if is_already_colliding:
                self.collision_exit(other)
                other.collision_exit(self)


    def check_pixel_collision(self, collision_box1, collision_box2, mask1, mask2):
        offset_x = collision_box2.x - collision_box1.x
        offset_y = collision_box2.y - collision_box1.y

        return mask1.overlap(mask2, (offset_x,offset_y)) is not None




    def start(self):
        pass

    def update(self, delta_time):
        pass

    def collision_enter(self, other):
        self._other_colliders.append(other)
        if "collision_enter" in self._listeners:
            self._listeners["collision_enter"](other)

    def collision_exit(self, other):
         self._other_colliders.remove(other)
         if "collision_exit" in self._listeners:
            self._listeners["collision_exit"](other)

    def pixel_collision_enter(self,other):
        self._other_masks.append(other)
        if "pixel_collision_enter" in self._listeners:
            self._listeners["pixel_collision_enter"](other)

    def pixel_collision_exit(self,other):
         self._other_masks.remove(other)
         if "pixel_collision_exit" in self._listeners:
            self._listeners["pixel_collision_exit"](other)