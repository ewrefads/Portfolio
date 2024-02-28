import pygame

def display_menu():
    print("Welcome to the Game Menu")
    print("1. Start Game")
    print("2. Load Game")
    print("3. Options")
    print("4. Quit")

def start_game():
    print("Starting the game...")

def load_game():
    print("Loading the game...")

def show_options():
    print("Displaying options...")

def quit_game():
    print("Quitting the game...")

# Main function to run the game
def main():
    while True:
        display_menu()
        choice = input("Enter your choice: ")

        if choice == "1":
            start_game()
        elif choice == "2":
            load_game()
        elif choice == "3":
            show_options()
        elif choice == "4":
            quit_game()
            break
        else:
            print("Invalid choice. Please select again.")

if __name__ == "__main__":
    main()
