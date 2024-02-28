import pygame
from Components import Component

class Entity(Component):

    _health = 0
    _speed = 0
    _default_speed = 0
    _deathDef = None
    starting_collider = None
    collider = None
    game_world = None
    ignore_collision = False
    floating = False
    alive = True
    final_fall = False
    direction = pygame.Vector2(0,0)
    total_fall_distance = 0

    def __init__(self, health, speed) -> None:
        super().__init__()
        self._health = health
        self._speed = speed
        self._default_speed = speed

    def awake(self, game_world):
        self.game_world = game_world
        self._deathDef = self._gameObject.destroy
        self.collider = self._gameObject.get_component("Collider")
        self.collider.subscribe("collision_exit", self.on_collision_exit)

    def start(self):
        return super().start()
    
    def update(self, delta_time):
        if self.final_fall == False:
            self.floating = True
            if self.ignore_collision == False:
                rect = self._gameObject.get_component("SpriteRenderer").sprite.rect.copy()
                r = rect.copy()
                r.y +=1
                shortest_distance = self.speed
                if self.speed < 5:
                    shortest_distance = 5
                for collider in self.game_world.colliders:
                    hazard = collider.gameObject.get_component("Hazard")
                    projectile = collider.gameObject.get_component("Projectile")
                    if(collider == None):
                        continue
                    if collider != self.collider and collider.collision_box.colliderect(r) and self.collider.collision_box.colliderect(collider.collision_box) == False and hazard == None and projectile == None:
                        self.floating = False
                        self.total_fall_distance = 0
                        if self.alive == False:
                            self.final_fall = True
                        break
                    elif collider.collision_box.top - rect.bottom < shortest_distance and collider.collision_box.top - rect.bottom > 0 and hazard == None and projectile == None:
                        shortest_distance = collider.collision_box.top - rect.bottom
                if self.floating:
                    self.direction.y = shortest_distance
                    self.total_fall_distance += abs(shortest_distance)
                if self.total_fall_distance > 500:
                    self.take_damage(50000)
        
        
    def sideways_collision(self):
        shortest_distance = self.direction.x
        if self.direction.x < 0:
            for collider in self.game_world.colliders:
                hazard = collider.gameObject.get_component("Hazard")
                projectile = collider.gameObject.get_component("Projectile")
                if hazard == None and projectile == None:
                    height_dif = self.collider.collision_box.bottom - collider.collision_box.bottom
                    if height_dif < 10 and height_dif > -10 and self.collider.collision_box.left - collider.collision_box.right < abs(shortest_distance) and collider.collision_box.right - self.collider.collision_box.left <= 0:
                        shortest_distance = collider.collision_box.right - self.collider.collision_box.left
                    elif self.floating or self.ignore_collision:
                        if height_dif < 80 and height_dif > -80 and self.collider.collision_box.left - collider.collision_box.right < abs(shortest_distance) and collider.collision_box.right - self.collider.collision_box.left <= 0:
                            shortest_distance = collider.collision_box.right - self.collider.collision_box.left
        elif self.direction.x > 0:
            for collider in self.game_world.colliders:
                hazard = collider.gameObject.get_component("Hazard")
                projectile = collider.gameObject.get_component("Projectile")
                if hazard == None and projectile == None:
                    height_dif = self.collider.collision_box.bottom - collider.collision_box.bottom
                    if height_dif < 10  and height_dif > -10 and collider.collision_box.left - self.collider.collision_box.right < shortest_distance and collider.collision_box.left - self.collider.collision_box.right > 0:
                        shortest_distance = collider.collision_box.left - self.collider.collision_box.right
                    elif self.floating or self.ignore_collision:
                        if height_dif < 80  and height_dif > -80 and collider.collision_box.left - self.collider.collision_box.right < shortest_distance and collider.collision_box.left - self.collider.collision_box.right > 0:
                                shortest_distance = collider.collision_box.left - self.collider.collision_box.right 
        self.direction.x = shortest_distance
        if shortest_distance < 5 and shortest_distance > -5:
            self.direction.x = 0
    def take_damage(self, damage):
        if self._health - damage <= 0 and self.alive:
            self.alive = False
            self._deathDef()
        else:
            self._health -= damage
    def change_speed(self, value):

        self._speed = value

    def reset_speed(self):
        self._speed = self._default_speed
    @property
    def health(self):
        return self.health
    
    @property
    def speed(self):
        return self._speed
    
    @property
    def deathDef(self):
        return self._deathDef
    
    def DeathDef(self, value):
        self._deathDef = value

    def on_collision_exit(self, other):
        if other.gameObject.get_component("Hazard") == None:
            self.ignore_collision = False
    