import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FooterComponent } from '../footer/footer.component';

interface FaqItem {
  question: string;
  answer: string;
}

interface FaqSection {
  title: string;
  items: FaqItem[];
}

@Component({
  selector: 'app-faq',
  standalone: true,
  imports: [CommonModule, RouterLink, FooterComponent],
  templateUrl: './faq.component.html',
  styleUrl: './faq.component.css'
})
export class FaqComponent {

  openItems = new Set<string>();

  toggle(key: string): void {
    if (this.openItems.has(key)) {
      this.openItems.delete(key);
    } else {
      this.openItems.add(key);
    }
  }

  isOpen(key: string): boolean {
    return this.openItems.has(key);
  }

  sections: FaqSection[] = [
    {
      title: 'Getting Started',
      items: [
        {
          question: 'What is ReactionLab?',
          answer: 'ReactionLab is an interactive learning platform for chemistry. It provides animated visualizations of chemical reactions and concepts, making it easier to understand what happens at the molecular level.'
        },
        {
          question: 'Do I need an account to use ReactionLab?',
          answer: 'You can browse the homepage and explore the library without an account. However, creating a free account unlocks progress tracking, badges, and personalized learning features.'
        },
        {
          question: 'Is ReactionLab free to use?',
          answer: 'Yes, ReactionLab is completely free. All visualizations and learning content are available at no cost once you create an account.'
        },
        {
          question: 'What devices and browsers are supported?',
          answer: 'ReactionLab works on any modern browser — Chrome, Firefox, Edge, and Safari. It is fully responsive and works on desktops, tablets, and mobile devices.'
        },
        {
          question: 'How do I create an account?',
          answer: 'Click the Sign up button in the top navigation bar, choose a display emoji, enter your name and password, and you\'re ready to go. No email verification required.'
        }
      ]
    },
    {
      title: 'Learning & Content',
      items: [
        {
          question: 'What topics does ReactionLab cover?',
          answer: 'ReactionLab covers a range of chemistry topics including acid-base reactions, oxidation and reduction, thermodynamics, organic reaction mechanisms, and more. New topics are added regularly.'
        },
        {
          question: 'How are the visualizations organized?',
          answer: 'Visualizations are grouped into topics. Each topic contains multiple interactive experiments you can step through at your own pace, with labels and annotations explaining what is happening at each stage.'
        },
        {
          question: 'Can I track my learning progress?',
          answer: 'Yes. Once logged in, your progress is saved automatically. You can see which topics and experiments you have completed from the My Learning page.'
        },
        {
          question: 'What are badges and how do I earn them?',
          answer: 'Badges are awarded when you complete a topic or reach a milestone. They appear on your profile and a notification dot will show on the My Learning link in the navigation bar when a new one is earned.'
        },
        {
          question: 'How often is new content added?',
          answer: 'New topics and visualizations are added on an ongoing basis. If there is a specific reaction or concept you would like to see covered, feel free to reach out via the Contacts page.'
        }
      ]
    },
    {
      title: 'Technical & Support',
      items: [
        {
          question: 'What should I do if a visualization is not loading?',
          answer: 'Try refreshing the page. If the issue persists, make sure your browser is up to date and that JavaScript is enabled. You can also reach us through the Contacts page and we will look into it.'
        },
        {
          question: 'Can I use ReactionLab offline?',
          answer: 'ReactionLab currently requires an internet connection to load visualizations and sync your progress. Offline support is not available at this time.'
        },
        {
          question: 'How do I reset my password?',
          answer: 'Password reset is not yet available through the interface. If you have lost access to your account, please contact us directly via the Contacts page and we will assist you manually.'
        },
        {
          question: 'How can I contact the team?',
          answer: 'You can reach us through the Contacts page, where you will find links to email, GitHub, and LinkedIn. We try to respond as quickly as possible.'
        }
      ]
    }
  ];
}
