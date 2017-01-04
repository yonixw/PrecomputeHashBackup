import traceback, sys

NO_TAG = "no tag"

def logError(tag=NO_TAG):
    exc_type, exc_value, exc_traceback = sys.exc_info()
    print("[ERROR/" + tag + "]")
    traceback.print_exception(exc_type, exc_value, exc_traceback, file=sys.stdout)

def logCustomError(message, stack, tag=NO_TAG):
    print("[ERROR/" + tag + "] Message:\n" + message + "StackTrace:\n" + stack)

def logDebug(text, tag=NO_TAG):
    print("[DEBUG/" + tag + "] " + text)

def log(text, tag=NO_TAG):
    logDebug(text, tag)

