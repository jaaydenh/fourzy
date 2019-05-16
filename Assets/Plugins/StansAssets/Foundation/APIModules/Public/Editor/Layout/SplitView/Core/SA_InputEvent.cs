using UnityEngine;

namespace SA.Foundation.Editor
{
    public class SA_InputEvent
    {
        private Event m_sourceEvent;

        public SA_InputEvent(Event sourceEvent) {
            m_sourceEvent = sourceEvent;
        }

        public int displayIndex {
            get {
                return m_sourceEvent.displayIndex;
            }
        }

        public Vector2 mousePosition {
            get {
                return m_sourceEvent.mousePosition;
            }
        }

        public Vector2 delta {
            get {
                return m_sourceEvent.delta;
            }
        }

        public bool shift {
            get {
                return m_sourceEvent.shift;
            }
        }

        public bool control {
            get {
                return m_sourceEvent.control;
            }
        }

        public bool command {
            get {
                return m_sourceEvent.command;
            }
        }

        public KeyCode keyCode {
            get {
                return m_sourceEvent.keyCode;
            }
        }

        public bool capsLock {
            get {
                return m_sourceEvent.capsLock;
            }
        }

        public bool numeric {
            get {
                return m_sourceEvent.numeric;
            }
        }

        public bool functionKey {
            get {
                return m_sourceEvent.functionKey;
            }
        }

        public bool isKey {
            get {
                return m_sourceEvent.isKey;
            }
        }

        public bool alt {
            get {
                return m_sourceEvent.alt;
            }
        }

        public string commandName {
            get {
                return m_sourceEvent.commandName;
            }
        }

        public int clickCount {
            get {
                return m_sourceEvent.clickCount;
            }
        }

        public bool isMouse {
            get {
                return m_sourceEvent.isMouse;
            }
        }

        public float pressure {
            get {
                return m_sourceEvent.pressure;
            }
        }

        public EventModifiers modifiers {
            get {
                return m_sourceEvent.modifiers;
            }
        }

        public int button {
            get {
                return m_sourceEvent.button;
            }
        }

        public EventType type {
            get {
                return m_sourceEvent.type;
            }
        }

        public EventType rawType {
            get {
                return m_sourceEvent.rawType;
            }
        }

        public char character {
            get {
                return m_sourceEvent.character;
            }
        }

        public bool isScrollWheel {
            get {
#if !UNITY_5
                return m_sourceEvent.isScrollWheel;
#else
                return true;
#endif

			}
		}

		public Event SourceEvent {
			get {
				return m_sourceEvent;
			}
		}

		public EventType GetTypeForControl(int controlID) {
			return m_sourceEvent.GetTypeForControl(controlID);
		}

		public override string ToString() {
			return m_sourceEvent.ToString();
		}

		//public override bool Equals(object obj);
		//public override int GetHashCode();
	}
}
