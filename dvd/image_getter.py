from tkinter import *
from PIL import Image, ImageTk


def get_image(active_tkinter):
    img = Image.open("images/ODDBALL.png").convert("RGBA")

    width, height = img.size
    new_size = (int(width * 0.5), int(height * 0.5))
    resized_image = img.resize(new_size)


    new_width, new_height = resized_image.size
    # Define crop box as a percentage of the new size
    left = int(new_width * 0.25)  # 10% from the left
    top = int(new_height * 0.05)  # 5% from the top
    right = new_width - int(new_width * 0.2)  # 10% from the right
    bottom = new_height - int(new_height * 0.05)  # 5% from the bottom

    border = (left, top, right, bottom)
    cropped_image = resized_image.crop(border)

    tk_image = ImageTk.PhotoImage(image=cropped_image, master=active_tkinter)
    return tk_image
