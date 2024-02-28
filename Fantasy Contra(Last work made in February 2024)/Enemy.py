import random
import pygame
from Components import Component, SpriteRenderer
import math
from Components import Component
from Entity import Entity
from GameObject import GameObject

class Enemy(Entity):
    range = 0
    damage = 0
    cooldown = 1
    remaining_cooldown = 0
    actions = []
    points_on_death = 0
    type = ""
    animator = None
    blocking = False
    remaining_wait = 0
    max_wait = 2
    def __init__(self, health, speed, range, damage, points, type) -> None:
        super().__init__(health, speed)
        self.range = range
        self.damage = damage
        self.points_on_death = points
        self.type = type

        if type == "flyeye":
            self.amplitude = 50
            self.angle = 0  # Start vinkel for oscillation


    def awake(self, game_world):
        super().awake(game_world)
        if self.type == "skeleton":
            self.actions.append(self.shield_block)
        if self.type != "flyeye":
            self.actions.append(self.move_toward_player)
        else:
            self.actions.append(self.flight_with_oscillation)
            
        self.actions.append(self.attack_player)
        self.DeathDef(self.on_death)
        self.animator = self.gameObject.get_component("Animator")
        self.animator.animation_finish_subscribe(self.animation_finished)

    def start(self):
        super().start()

    def update(self, delta_time):
        if self.alive:
            super().update(delta_time)
            for action in self.actions:
                if action():
                    break
            if self.remaining_cooldown > 0:
                self.remaining_cooldown -= delta_time
                print(self.remaining_cooldown)

            if self.remaining_wait > 0:
                self.remaining_wait -= delta_time
                
            old_distance = self.direction.x
            #moved = False
            #if self.direction.x != 0:
                #moved = True
            #self.sideways_collision()
            #if moved:
                #self.animator.play_animation("idle")
            if self.direction.x > self.speed:
                self.direction.x = self.speed
            elif self.direction.x < -self.speed:
                self.direction.x = -self.speed
            self._gameObject.transform.translate(self.direction)
            self.direction = pygame.Vector2(0,0)
            self.reset_speed()
        elif self.animator.current_animation != "death" or self.animator.current_animation != "dead":
            self.animator.play_animation("death")
    
    def move_toward_player(self):
        height_dif = self.game_world.player.gameObject.transform.position.y - self._gameObject.transform.position.y
        if height_dif < 80 and height_dif > -80 and self.alive:
            distance_dif = self.game_world.player.gameObject.transform.position.x - self._gameObject.transform.position.x
            distance = 0
            delta_x = 0
            if distance_dif > 0:
                distance = self.game_world.player.collider.collision_box.right - self.gameObject.get_component("Collider").collision_box.left
                delta_x = 1
            else:
                delta_x = -1
                distance = self.gameObject.get_component("Collider").collision_box.left - self.game_world.player.collider.collision_box.right
            delta_x = self.speed * delta_x
            if distance < self.speed or distance <= self.range:
                return False
            elif self.animator.current_animation != "hit" and self.animator.current_animation != "shield_block" and abs(distance_dif < 200):
                self.direction.x += delta_x
                if self.type == "goblin" or self.type == "skeleton" or self.type == "mushroom" and self.alive:
                    self.animator.play_animation("run")
                if delta_x <= 0:
                    self.gameObject.get_component("SpriteRenderer").flipped = True
                else:
                    self.gameObject.get_component("SpriteRenderer").flipped = False
                return True
        else: 
            return False
        
    def flight_with_oscillation(self): # Movement method for flyeye
        self._gameObject.transform.position.y = 100 + self.amplitude * math.sin(self.angle)
        self.angle += self.speed
        self._gameObject.transform.position.x += self.direction.x * self.speed
        

    def attack_player(self):
        
        height_dif = self.game_world.player.gameObject.transform.position.y - self._gameObject.transform.position.y
        if height_dif < 80 and height_dif > -80 and self.alive:
            distance_dif = self.game_world.player.gameObject.transform.position.x - self._gameObject.transform.position.x
            direction = 0
            if distance_dif < 0:
                distance_dif = self.game_world.player.collider.collision_box.right - self.gameObject.get_component("Collider").collision_box.left
                direction = -1
            else:
                direction = 1
            offset = 0
            if distance_dif > self.range:
                delta_x = distance_dif * direction     
                self.direction.x = delta_x
                return True
            elif abs(distance_dif) <= self.range and self.remaining_cooldown <= 0 and self.animator.current_animation != "attack_prepare":
                self.animator.play_animation("attack_prepare")
                return True
            else:
                return False
        else:
            return False
    
    def take_damage(self, damage):
        if self.blocking == False and self.alive:
            super().take_damage(damage)
            if self.type == "goblin" or self.type == "skeleton" or self.type == "mushroom" and self.alive:
                self.animator.play_animation("hit")
        elif self.alive:
            self.game_world.AudioManager.play_sfx("shield_block")
            self.blocking = False
            self.animator.play_animation("idle")

    def on_death(self):
        print("dying")
        if self.alive == False:
            self.alive = False
        self.game_world.update_score(self.points_on_death)
        self.animator.play_animation("death")

    def animation_finished(self, animation):
        if animation == "attack_prepare":
            if self.alive:
                if self.type == "goblin" or self.type == "skeleton":
                    self.game_world.AudioManager.play_sfx("sword_hit")
                elif self.type == "mushroom":
                    self.game_world.AudioManager.play_sfx("punch")
                self.remaining_cooldown = self.cooldown
                self.game_world.player.take_damage(self.damage)
                print("damaged player")
                self.animator.play_animation("attack_aftermath")
            else:
                self.animator.play_animation("death")
        elif animation == "attack_aftermath":
            self.animator.play_animation("idle")
        elif animation == "hit":
            self.animator.play_animation("idle")
        elif animation == "death":
            print("dead")
            if self.type == "goblin":
                dead_goblin = GameObject(pygame.Vector2(self.gameObject.transform.position.x, self.gameObject.get_component("SpriteRenderer").sprite.rect.top + self.gameObject.get_component("SpriteRenderer").sprite.rect.height - 20))
                sr = dead_goblin.add_component(SpriteRenderer("Goblin/Death/tile003.png"))
                sr.flipped = self.gameObject.get_component("SpriteRenderer").flipped
                self.game_world.instantiate(dead_goblin)
            self.gameObject.destroy()
        elif self.alive == False and animation == "run":
            self.animator.play_animation("death")
    def shield_block(self):
        if self.game_world.player.active_projectile != "" and self.blocking == False and self.remaining_cooldown <= 0:
            block = random.randint(1, 2)
            print(block)
            if block == 1:
                self.blocking = True
                self.remaining_cooldown = self.cooldown
                self.remaining_wait = self.max_wait
                self.animator.play_animation("shield_block")
                return True
            else:
                self.remaining_cooldown = self.cooldown
                return False
        elif self.remaining_wait <= 0 and self.blocking:
            self.blocking = False
            self.animator.play_animation("idle")
            self.remaining_wait = 0
        else:
            return False