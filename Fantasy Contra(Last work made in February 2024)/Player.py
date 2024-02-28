import pygame
from ProjectileFactory import ProjectileFactory
from Entity import Entity
from Projectile import Projectile

class Player(Entity):

    repeats = 0
    mana = 15
    max_mana = 15
    
    animator = None
    game_world = None
    total_projectiles = 0
    projectile_direction = pygame.Vector2(1, 0)
    projectile_factory = ProjectileFactory()
    active_cooldowns = {}
    actions = {}
    height_increase = 0
    mana_cooldown = 0.5
    current_mana_cooldown = 0
    active_projectile = ""
    left_or_right = True
    step_delay = 0
    max_step_delay = 0.15
    #initializer for at kunne få reference til playerens HUD
    def __init__(self, health, speed, mana, damage_multiplier) -> None:
        super().__init__(health, speed)
        self._player_HUD = None
        self.mana = mana
        self.damage_multiplier = damage_multiplier
        

    def awake(self, game_world):
        super().awake(game_world)
        self.animator = self._gameObject.get_component("Animator")
        self.animator.animation_finish_subscribe(self.finished_animation)
        self.bind_action(pygame.K_1, self.firebolt)
        self.active_cooldowns["firebolt"] = 0
        self.bind_action(pygame.K_2, self.frostbolt)
        self.active_cooldowns["frostbolt"] = 0
    def start(self):
        super().start()
        
        #spilleren får hud reference fra gameworld og sætter HUD op med player variables
        self._player_HUD = self.game_world._HUD
        self._player_HUD.max_mana_change(self.max_mana)
        self._player_HUD.mana_change(self.mana)
        self._player_HUD.health_change(self._health)
        
        pass
    def update(self, delta_time):
        super().update(delta_time)
        keys = pygame.key.get_pressed()
        if keys[pygame.K_w] and self.ignore_collision == False and self.floating == False:
            self.ignore_collision = True
            self.repeats = 2
            self.animator.play_animation("jump")
        if keys[pygame.K_a]:
            self.direction.x -= self.speed
            self.projectile_direction.x = -1
        if keys[pygame.K_d]:
            self.direction.x += self.speed
            self.projectile_direction.x = 1
        if keys[pygame.K_LCTRL]:
            self.direction.y += self.speed
        if keys[pygame.K_ESCAPE]:
            pygame.quit()
        if self.repeats > 0:
            self.direction.y -= self.speed
            
            if self.height_increase > 160:
                self.ignore_collision = False
                self.height_increase = 0
            elif self.ignore_collision:
                self.height_increase += self.speed
            if self.ignore_collision == False:
                self.repeats -= 1
            if self.repeats == 0:
                self.ignore_collision = False
        self.sideways_collision()
        self._gameObject.transform.translate(self.direction)
        if self.step_delay > 0:
            self.step_delay -= delta_time
        if self.direction.x == 0 and self.direction.y == 0 and self.active_projectile == "":
            self.animator.play_animation("idle")
        elif self.floating  and self.animator.current_animation != "attack2":
            if self.direction.x < 0:
                self._gameObject.get_component("SpriteRenderer").flipped = True
            if self.animator.current_animation != "jump":
                self.animator.play_animation("fall")
        elif self.direction.x > 0  and self.animator.current_animation != "attack2":
            self.animator.play_animation("run")
            if self.left_or_right and self.step_delay <= 0:
                self.game_world.AudioManager.play_sfx("footstep0")
                self.left_or_right = False
                self.step_delay = self.max_step_delay
            elif self.step_delay <= 0:
                self.game_world.AudioManager.play_sfx("footstep1")
                self.left_or_right = True
                self.step_delay = self.max_step_delay
            self._gameObject.get_component("SpriteRenderer").flipped = False
        elif self.direction.x < 0  and self.animator.current_animation != "attack2":
            self._gameObject.get_component("SpriteRenderer").flipped = True
            self.animator.play_animation("run")
            if self.left_or_right and self.step_delay <= 0:
                self.game_world.AudioManager.play_sfx("footstep0")
                self.left_or_right = False
                self.step_delay = self.max_step_delay
            elif self.step_delay <= 0:
                self.game_world.AudioManager.play_sfx("footstep1")
                self.left_or_right = True
                self.step_delay = self.max_step_delay
        for key in self.actions:
            if keys[key] and self.floating == False:
                self.actions[key]()
        for action in self.active_cooldowns:
            if self.active_cooldowns[action] > 0:
                self.active_cooldowns[action] -= delta_time
        if self.mana < self.max_mana and self.current_mana_cooldown <= 0:
            self.mana += 2
            self.current_mana_cooldown = self.mana_cooldown
            self.game_world.update_mana(self.mana)
        else:
            self.current_mana_cooldown -= delta_time
        self.direction = pygame.Vector2(0,0)
        self.reset_speed()

    def bind_action(self, key, action):
        self.actions[key] = action

    

    def firebolt(self):
        if self.active_cooldowns["firebolt"] <= 0 and self.mana >= 6:
            self.game_world.AudioManager.play_sfx("magic_spell")
            self.mana -= 5
            self._player_HUD.mana_change(self.mana)
            self.animator.play_animation("attack2prepare")
            self.animator.frame_duration = 0.01
            self.active_cooldowns["firebolt"] = 0.5
            self.active_projectile = "firebolt"

    def firebolt_instantiate(self):
        firebolt = self.projectile_factory.Create("firebolt")
        self.total_projectiles += 1
        firebolt.transform.position = self._gameObject.transform.position + pygame.Vector2(45 * self.projectile_direction, 10) 
        firebolt.add_component(Projectile(self.projectile_direction, int(5 * self.damage_multiplier), self.collider, False, None, "firebolt"))
        self.game_world.instantiate(firebolt)
    def frostbolt(self):
        if self.active_cooldowns["frostbolt"] == 0 and self.mana >= 6:
            self.game_world.AudioManager.play_sfx("magic_spell")
            self.mana -= 6
            self.active_cooldowns["frostbolt"] = 0.5
            self._player_HUD.mana_change(self.mana)
            self.animator.play_animation("attack2prepare")
            self.animator.frame_duration = 0.01
            self.active_projectile = "frostbolt"
            
    def frostbolt_instantiate(self):
        frostbolt = self.projectile_factory.Create("frostbolt")
        self.total_projectiles += 1
        frostbolt.transform.position = self._gameObject.transform.position + pygame.Vector2(45 * self.projectile_direction, 10) 
        frostbolt.add_component(Projectile(self.projectile_direction, int(2 * self.damage_multiplier), self.collider, True, "frostbolt", "frostbolt"))
        self.game_world.instantiate(frostbolt)
    def take_damage(self, damage):
        dead = False
        if self._health - damage <= 0:
            dead = True
        super().take_damage(damage)
        
        #opdatere helbreds ui
        self._player_HUD.health_change(round(self._health))
        
        if dead:
            self.game_world.game_over()

    def finished_animation(self, animation):
        if animation == "jump":
            self.animator.play_animation("fall")
        if animation == "attack2prepare":
            print("preparation finished")
            self.animator.play_animation("attack2aftermath")
            if self.active_projectile == "firebolt":
                self.firebolt_instantiate()
            elif self.active_projectile == "frostbolt":
                self.frostbolt_instantiate()
        if animation == "attack2aftermath":
            self.animator.play_animation("idle")
            self.animator.frame_duration = 0.1
            self.active_projectile = ""