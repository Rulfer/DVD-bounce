from tkinter import *
from PIL import Image, ImageTk


def get_image(active_tkinter):
    img = Image.open("images/ODDBALL.png")
    width, height = img.size
    new_width = 200
    new_height = 100
    left = (width - new_width) / 2
    top = (height - new_height) / 2
    right = (width + new_width) / 2
    bottom = (height + new_height) / 2

    border = (left, top, right, bottom)
    cropped_image = img.crop(border)

    if cropped_image.mode != 'RGBA':
        cropped_image = cropped_image.convert('RGBA')

    transparent_background = Image.new('RGBA', cropped_image.size, (0, 0, 0, 0))
    transparent_background.paste(cropped_image, (0, 0), cropped_image)

    tk_image = ImageTk.PhotoImage(image=transparent_background, master=active_tkinter)
    return tk_image
