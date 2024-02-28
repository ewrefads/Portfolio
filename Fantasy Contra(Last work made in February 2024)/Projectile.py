import pygame
from Components import Component


class Projectile(Component):

    damage = 0

    direction = pygame.Vector2(0,0)
    max = pygame.Vector2(0,0)
    type = None
    owner_collider = None
    apply_effect = False
    effect_type = None
    hit_target = False
    target = None
    game_world = None
    
    def __init__(self, direction, damage, collider, apply_effect, effect_type, type) -> None:
        super().__init__()
        self.direction = pygame.Vector2(direction.x, direction.y)
        self.damage = damage
        self.owner_collider = collider
        self.apply_effect = apply_effect
        if self.apply_effect:
            self.effect_type = effect_type
        self.type = type

    def awake(self, game_world):
        self.game_world = game_world
        self.max.x = game_world.screen.get_width()
        self.max.y = game_world.screen.get_height()
        self._gameObject.get_component("Collider").subscribe("pixel_collision_enter", self.on_hit)
        if self.direction.x < 0:
            self.gameObject.get_component("SpriteRenderer").flipped = True
        if self.apply_effect:
            if self.effect_type == "frostbolt":
                self.active_effect = self.frostbolt

    def start(self):
        pass

    def update(self, delta_time):
        if self.hit_target == False:
            speed = 1000
            movement = pygame.math.Vector2(self.direction.x * speed,self.direction.y * speed)
            camera_offset = self.game_world.camera_offset
            self._gameObject.transform.translate(movement*delta_time)

            if self._gameObject.transform.position.y < 0 + camera_offset[1] or self._gameObject.transform.position.x < 0 + camera_offset[0] or self._gameObject.transform.position.y > self.max.y + camera_offset[1] or self._gameObject.transform.position.x > self.max.x + camera_offset[0]:
                print("noget sejt")
                self._gameObject.destroy()
        elif self.target != None:
            self.active_effect(self.target, delta_time)
        else:
            self.gameObject.destroy()
        
    
    def on_hit(self, other):
        
        if other != self.owner_collider and other.gameObject.get_component("Projectile") == None:
            if other.gameObject.get_component("Enemy") != None:
                if self.type == "firebolt":
                    self.game_world.AudioManager.play_sfx("fire_impact")
                elif self.type == "frostbolt":
                    self.game_world.AudioManager.play_sfx("ice_impact")
                other.gameObject.get_component("Enemy").take_damage(self.damage)
                self.damage = 0
                if self.apply_effect == False:
                    self._gameObject.destroy()
                else:
                    self.hit_target = True
                    self._gameObject.get_component("SpriteRenderer").render = False
                    self.target = other.gameObject.get_component("Enemy")
                    self.duration = 5
            else:
                print("hit terrain")
                self._gameObject.destroy()
                

    def frostbolt(self, other, delta_time):
        if self.duration - delta_time > 0:
            other.change_speed(other.speed * 0.5)
            self.duration -= delta_time
