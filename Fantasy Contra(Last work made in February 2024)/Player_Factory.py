from Components import Animator, Collider, SpriteRenderer
from Factory import Factory
from GameObject import GameObject
import pygame

from Player import Player


class PlayerFactory(Factory):

    health = 10
    speed = 10
    mana = 15
    damage_mul = 1

    def __init__(self) -> None:
        super().__init__()
    
    def Create(self, type):
        player = GameObject(pygame.Vector2(0,0))
        player_spriteRenderer = player.add_component(SpriteRenderer("/wizard/Idle/tile000.png"))
        player_animator = player.add_component(Animator())
        idle_prefix = "/wizard/Idle/"
        idle_animation = [f"{idle_prefix}tile000.png", f"{idle_prefix}tile001.png", f"{idle_prefix}tile002.png", f"{idle_prefix}tile003.png", f"{idle_prefix}tile004.png", f"{idle_prefix}tile005.png"]
        player_animator.add_animation("idle", idle_animation[0], idle_animation[1], idle_animation[2], idle_animation[3], idle_animation[4], idle_animation[5])
        player_animator.play_animation("idle")
        run_prefix = "wizard/run/"
        run_animation = [f"{run_prefix}tile000.png", f"{run_prefix}tile001.png", f"{run_prefix}tile002.png", f"{run_prefix}tile003.png", f"{run_prefix}tile004.png", f"{run_prefix}tile005.png", f"{run_prefix}tile006.png", f"{run_prefix}tile007.png"]
        player_animator.add_animation("run", run_animation[0], run_animation[1], run_animation[2], run_animation[3], run_animation[4], run_animation[5], run_animation[6], run_animation[7])
        jump_prefix = "wizard/jump/"
        jump_animation = [f"{jump_prefix}tile000.png", f"{jump_prefix}tile001.png"]
        player_animator.add_animation("jump", jump_animation[0], jump_animation[1])
        fall_prefix = "wizard/Fall/"
        fall_animation = [f"{fall_prefix}tile000.png", f"{fall_prefix}tile001.png"]
        player_animator.add_animation("fall", fall_animation[0], fall_animation[1])

        attack2_prepare_prefix = "wizard/Attack2/Prepare/"
        attack2_animation = [f"{attack2_prepare_prefix}tile000.png", f"{attack2_prepare_prefix}tile001.png", f"{attack2_prepare_prefix}tile002.png", f"{attack2_prepare_prefix}tile003.png", f"{attack2_prepare_prefix}tile004.png"]
        player_animator.add_animation("attack2prepare", attack2_animation[0], attack2_animation[1], attack2_animation[2], attack2_animation[3], attack2_animation[4])
        attack2_aftermath_prefix = "wizard/Attack2/aftermath/"
        attack2_aftermath_animation = [f"{attack2_aftermath_prefix}tile005.png", f"{attack2_aftermath_prefix}tile006.png", f"{attack2_aftermath_prefix}tile007.png"]
        player_animator.add_animation("attack2aftermath", attack2_aftermath_animation[0], attack2_aftermath_animation[1], attack2_aftermath_animation[2])
        player.add_component(Collider())
        player.add_component(Player(self.health, self.speed, self.mana, self.damage_mul))
        return player