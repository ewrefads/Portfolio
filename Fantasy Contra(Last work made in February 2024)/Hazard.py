from Components import Component

class Hazard(Component):
    damage = 0
    speed_reduction = 0
    effect_time = 0
    remaining_time = 0
    active = False
    game_world = None
    one_time = False
    def __init__(self, damage, speed_reduction, effect_time, one_time) -> None:
        super().__init__()
        self.damage = damage
        self.speed_reduction = speed_reduction
        self.effect_time = effect_time
        self.one_time = one_time

    def awake(self, game_world):
        self.game_world = game_world
        collider = self.gameObject.get_component("Collider")
        collider.subscribe("pixel_collision_enter", self.on_collision)
        collider.subscribe("pixel_collision_exit", self.on_exit)
    
    def start(self):
        pass
    
    def update(self, delta_time):
        if self.active:
            if self.remaining_time > 0 or self.effect_time == 0:
                if self.remaining_time > 0:
                    self.remaining_time -= delta_time
                    self.apply_effects(delta_time)
    def apply_effects(self, delta_time):
        player = self.game_world.player

        player.take_damage(self.damage * delta_time)
        if self.speed_reduction is not bool:
            print("reducing speed")
            if self.speed_reduction % 1 != 0 and self.speed_reduction > 0:
                player.change_speed(player.speed * self.speed_reduction)
            elif self.speed_reduction == 0:
                player.change_speed(0)
            else:
                player.change_speed(player.speed - self.speed_reduction)

        if self.effect_time > 0 and self.remaining_time == 0:
            self.active = False
            if self.one_time:
                self._gameObject.destroy
        elif self.one_time and self.effect_time == 0:
            self._gameObject.destroy
    def on_collision(self, other):
        if(self.game_world.player is None):
            return
        
        player_collider = self.game_world.player.gameObject.get_component("Collider")
        if other == player_collider:
            if self.effect_time > 0:
                self.remaining_time = self.effect_time
            self.active = True
    def on_exit(self, other):
        player_collider = self.game_world.player.gameObject.get_component("Collider")
        if other == player_collider:
            self.active = False