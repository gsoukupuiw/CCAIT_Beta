# import the opencv library
import cv2
  

cap = cv2.VideoCapture(0)


cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)


while(cap.isOpened()):


    ret, frame = cap.read()
    


    cv2.imshow('frame',frame)
    cv2.waitKey(10)
    
cap.release()
cv2.destroyAllWindows()
