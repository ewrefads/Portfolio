from Components import Component
import pygame

class Explosion(Component):
    explosion_range = 0
    explosion_power = 0
    remaining_power = 0
    affected_entities = []
    location = pygame.Vector2(0,0)
    active_explosion = False
    started_explosion = False
    def __init__(self, explosion_range, explosion_power) -> None:
        super().__init__()
        self.explosion_range = explosion_range
        self.explosion_power = explosion_power
    
    def awake(self, game_world):
        self.game_world = game_world

    def start(self):
        pass

    def update(self, delta_time):
        if self.active_explosion:
            affected_entity = False
            for entity in self.affected_entities:
                print("applying effect to entity")
                change = pygame.Vector2(0,0)
                if self.remaining_power > 0:
                    affected_entity = True
                    change.y -= 5
                floating = False
                if entity.get_component("Enemy") != None:
                    floating = entity.get_component("Enemy").floating
                else:
                    floating = entity.get_component("Player").floating
                if self.remaining_power > 0 or floating:
                    affected_entity = True
                    if entity.transform.position.x > self.location.x:
                        change.x += 5
                    else:
                        change.x -= 5
                entity.transform.translate(change)
            if self.remaining_power > 0:
                self.remaining_power -= 1
            else:
                print("ran out of power")
            if affected_entity == False:
                self.active_explosion = False

    def start_explosion(self, location):
        print("started explosion")
        self.active_explosion = True
        self.started_explosion = True
        self.location = location
        self.remaining_power = self.explosion_power
        for collider in self.game_world.colliders:
            if collider.gameObject.get_component("Enemy") != None or collider.gameObject.get_component("Player") != None:
                height_dif = location.y - collider.gameObject.transform.position.y
                distance = location.x - collider.gameObject.transform.position.x
                if abs(height_dif) < 80 and abs(distance) <= self.explosion_range:
                    print("found valid collider")
                    self.affected_entities.append(collider.gameObject)

    def prepare_explosion(self):
        self.started_explosion = False