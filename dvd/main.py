from tkinter import *
import time

tk = Tk()
tk.overrideredirect("True")
canvas = Canvas(tk, bg="grey")

# Get screen width and height
screen_width = tk.winfo_screenwidth()
screen_height = tk.winfo_screenheight()


img = PhotoImage(file='F:/Personlig/dvd/images/cursed.png', master=tk)
img_label = Label(
    master=tk,
    image=img,
    bg="black",
    width=img.width(),
    height=img.height()
)
img_label.place(x=0, y=0)
img_label.pack()
xPos = round(screen_width / 2)
yPos = round(screen_height / 2)
xVel = 250
yVel = 250
moveRight = True
moveUp = True

# initialize time
previous_time = time.time()

def move_image():
    global xPos, yPos, previous_time, xVel, yVel
    current_time = time.time()
    delta_time = current_time - previous_time
    previous_time = current_time

    print(f"x is {xPos}")
    xPos += round(xVel * delta_time)
    yPos += round(yVel * delta_time)

    if xPos <= 0 or xPos + img.width() >= screen_width:
        xVel = -xVel
    if yPos <= 0 or yPos + img.height() >= screen_height:
        yVel = -yVel  # Reverse y velocity

    tk.geometry(f'{img.width()}x{img.height()}+{xPos}+{yPos}')

    # Update label position
    img_label.place(x=0, y=0)

    tk.after(10, move_image)

move_image()
tk.geometry(f'{img.height()}x{img.width()}+{xPos}+{yPos}')
tk.mainloop()
