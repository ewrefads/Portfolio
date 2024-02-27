import pygame
from Components import Animator, Collider, SpriteRenderer
from Enemy import Enemy
from Factory import Factory
from GameObject import GameObject


class MushroomFactory(Factory):
    def __init__(self) -> None:
        super().__init__()

    def Create(self, type):
        mushroom = GameObject(pygame.Vector2(0,0))
        asset_prefix = "Mushroom/"
        mushroom.add_component(SpriteRenderer(f"{asset_prefix}Idle/tile000.png"))
        animator = mushroom.add_component(Animator())
        mushroom.add_component(Collider())

        idle_prefix = f"{asset_prefix}Idle/"

        idle_animation = [f"{idle_prefix}tile000.png", f"{idle_prefix}tile001.png", f"{idle_prefix}tile002.png", f"{idle_prefix}tile003.png"]
        animator.add_animation("idle", idle_animation[0], idle_animation[1], idle_animation[2], idle_animation[3])
        animator.play_animation("idle")

        run_prefix = f"{asset_prefix}Run/"
        run_animation = [f"{run_prefix}tile000.png", f"{run_prefix}tile001.png", f"{run_prefix}tile002.png", f"{run_prefix}tile003.png", f"{run_prefix}tile004.png", f"{run_prefix}tile005.png", f"{run_prefix}tile006.png", f"{run_prefix}tile007.png"]
        animator.add_animation("run", run_animation[0], run_animation[1], run_animation[2], run_animation[3], run_animation[4], run_animation[5], run_animation[6], run_animation[7])

        attack_prefix = f"{asset_prefix}Attack/"
        attack_animation = [f"{attack_prefix}tile000.png", f"{attack_prefix}tile001.png", f"{attack_prefix}tile002.png", f"{attack_prefix}tile003.png", f"{attack_prefix}tile004.png", f"{attack_prefix}tile005.png", f"{attack_prefix}tile006.png", f"{attack_prefix}tile007.png"]
        animator.add_animation("attack_prepare", attack_animation[0], attack_animation[1], attack_animation[2], attack_animation[3], attack_animation[4], attack_animation[5], attack_animation[6])
        animator.add_animation("attack_aftermath", attack_animation[7])

        hit_prefix = f"{asset_prefix}Take Hit/"
        hit_animation = [f"{hit_prefix}tile000.png", f"{hit_prefix}tile001.png", f"{hit_prefix}tile002.png", f"{hit_prefix}tile003.png"]
        animator.add_animation("hit", hit_animation[0], hit_animation[1], hit_animation[2], hit_animation[3])

        death_prefix = f"{asset_prefix}Death/"
        death_animation = [f"{death_prefix}tile000.png", f"{death_prefix}tile001.png", f"{death_prefix}tile002.png", f"{death_prefix}tile003.png"]
        animator.add_animation("death", death_animation[0], death_animation[1], death_animation[2], death_animation[3])
        animator.add_animation("dead", death_animation[3])

        if type == "level 0":
            mushroom.add_component(Enemy(10, 6, 2, 1, 1, "mushroom"))
        elif type == "boss":
            mushroom.add_component(Enemy(20, 8, 2, 4, 10, "mushroom"))
        return mushroom