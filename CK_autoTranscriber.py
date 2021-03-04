import tkinter as tk
from tkinter import filedialog as fd

import os, json, editSpeakers

def browseFiles():
    global filename
    filename = fd.askopenfilename(initialdir = os.getcwd, title = "Select a File", filetypes = (("Text files", "*.txt*"), ("all files", "*.*")))

    label_file_explorer.configure(text=f"File Opened: {filename}")

    button_convert = tk.Button(window, text="Convert File", command=renpyConvertor)
    button_convert.place(anchor="center", relx=0.5, rely=0.26)

def openSpeakers():
    with open("speakers.json", "r") as f:
        return json.load(f)

def renpyConvertor():

    with open(filename, "r") as f:
        lines = f.readlines()

    rpy_file = f"{filename[:-4]}.rpy"

    with open(rpy_file, "w") as f:
        speakers = openSpeakers()
        hasSpeaker = False
        
        for line in lines:
            line = line.strip()

            if not line:
                f.write("\n")
                continue

            if line[0] == "-" and line[-1] == "-":
                f.write(f"# {line}\n")
                continue

            if line.startswith("NEW Message"):
                array = line.split()[2:]
                message = " ".join(array[1:])
                f.write(f'### $ contact_{array[0]}.newMessage("{message}")\n')
                continue

            if line.startswith("NEW ImageMessage"):
                array = line.split()[2:]
                imageDesc = " ".join(array[1:])
                f.write(f'### $ contact_{array[0]}.newImgMessage(**{imageDesc}**)\n')
                continue

            if line.startswith("NEW Reply"):
                array = line.split()[2:]
                message = " ".join(array[1:])
                f.write(f'### $ contact_{array[0]}.addReply("{message}")\n')
                continue

            if line.startswith("NEW KiwiiPost"):
                array = line.split()[2:]
                caption = " ".join(array[1:])
                f.write(f'### $ newKiwiiPost = KiwiiPost("{array[0]}", **"IMAGE"**, "{caption}", **numberLikes=999**)\n')
                continue

            if line.startswith("NEW KiwiiComment"):
                array = line.split()[2:]
                text = " ".join(array[1:])
                f.write(f'### $ newKiwiiPost.addComment("{array[0]}", "{text}", **numberLikes=999**)\n')
                continue

            if line.startswith("NEW KiwiiReply"):
                reply = line[15:]
                f.write(f'### $ newKiwiiPost.addReply("{reply}", **"replyLabel"**, **numberLikes=999**)\n')
                continue

            if line in speakers:
                hasSpeaker = True
                speaker = speakers[line]
                continue

            if hasSpeaker:
                f.write('{} "{}"\n'.format(speaker, line.replace("\"", "\\\"")))
                f.write("\nscene v8s\nwith dissolve\n")
            else:
                f.write(f"### ERROR: {line} ###\n")

            hasSpeaker = False

        successfulConvert(rpy_file)

def successfulConvert(file):
    label_Infomation.configure(text=f"File Successfully Converted.\nNew File: {file}")

if __name__ == "__main__":
    window = tk.Tk()
    window.title("Oscar's Text to Renpy Converter")
    window.geometry("500x500")
    window.config(background="white")

    label_file_explorer = tk.Label(window, text="File Explorer", width=100, height=4, fg="blue")

    button_explore = tk.Button(window, text="Browse Files", command=browseFiles)

    label_Infomation = tk.Label(window, text="", width=100, height=4, fg="blue")

    label_file_explorer.place(anchor="center", relx=0.5, rely=0.1)

    button_explore.place(anchor="center", relx=0.5, rely=0.2)

    label_Infomation.place(anchor="center", relx=0.5, rely=0.4)

    button_editSpeakers = tk.Button(window, text="Edit Speakers", command=editSpeakers.editSpeakers)
    button_editSpeakers.place(anchor="center", relx=0.5, rely=0.5)

    window.mainloop()

